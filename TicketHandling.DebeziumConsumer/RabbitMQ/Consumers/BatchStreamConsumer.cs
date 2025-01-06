using System.Text;
using System.Text.Json;
using RabbitMQ.Stream.Client;
using TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;

namespace TicketHandling.DebeziumConsumer.RabbitMQ.Consumers;

public class BatchStreamConsumer<TMessage> : ICustomConsumer
{
    private readonly StreamSystem _streamSystem;
    private readonly string _reference;
    private readonly string _streamName;

    private readonly JsonSerializerOptions _jsonSerializerOptions;
    
    private RawConsumer _consumer = null!;
    private ulong _currentOffset;
    
    private Message[] _buffer;
    private readonly object _lock = new();
    private readonly int _maxBatchSize;
    private int _currentBufferIndex;
    private readonly int _intervalMilliseconds;
    
    private ulong _status;

    IMessageHandler<TMessage[]>[] _handlers;
    
    public BatchStreamConsumer(
        StreamSystem streamSystem, 
        string reference, 
        string streamName, 
        int batchSize, 
        int intervalMilliseconds, 
        IMessageHandler<TMessage[]>[] handlers)
    {
        _streamSystem = streamSystem;

        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        
        _buffer = new Message[batchSize];
        _currentBufferIndex = 0;
        _maxBatchSize = batchSize;
        _intervalMilliseconds = intervalMilliseconds;
        _currentOffset = 0;
        
        _reference = reference;
        _streamName = streamName;
        
        _handlers = handlers;
    }

    public bool IsRunning => Interlocked.Read(ref _status) == 1;
    
    public async Task StartAsync()
    {
        IOffsetType offsetSpecification;
        try 
        {
            _currentOffset = await _streamSystem.QueryOffset(_reference, _streamName).ConfigureAwait(false);
            offsetSpecification = new OffsetTypeOffset(_currentOffset + 1);
        } 
        catch (OffsetNotFoundException)
        {
            _currentOffset = 0;
            offsetSpecification = new OffsetTypeFirst();
        }
        
        var consumerConfig = new RawConsumerConfig(_streamName)
        {
            Reference = _reference,
            OffsetSpec = offsetSpecification,
            MessageHandler = HandleMessage,
        };

        _consumer = await _streamSystem.CreateRawConsumer(consumerConfig) as RawConsumer 
                    ?? throw new NullReferenceException();

        if (_intervalMilliseconds != 0)
        {
            Task.Run(HandlerTimer);
        }
        
        Interlocked.Exchange(ref _status, 1);
    }

    public async Task StopAsync()
    {
        await _consumer.Close();
        Interlocked.Exchange(ref _status, 0);
    }
    
    private async Task HandleMessage(RawConsumer rawConsumer, MessageContext context, Message message)
    {
        int index;
        
        lock (_lock)
        {
            _buffer[_currentBufferIndex] = message;
            _currentBufferIndex++;
            _currentOffset = context.Offset;
            
            index = _currentBufferIndex;
        }

        if (index == _maxBatchSize)
            await FlushBuffer();
    }

    private async void HandlerTimer()
    {
        while (true)
        {
            await Task.Delay(_intervalMilliseconds);
            if (Interlocked.Read(ref _status) == 0) return;
            
            await FlushBuffer();
        }
    }
    
    private async Task FlushBuffer()
    {
        Message[] buffer;
        int length;
        ulong offset;
        
        lock (_lock)
        {
            buffer = _buffer;
            length = _currentBufferIndex;
            offset = _currentOffset;
            
            _buffer = new Message[_maxBatchSize];
            _currentBufferIndex = 0;
        }

        if (length == 0) return;

        TMessage[] batch = buffer
            .Take(length)
            .Select(x =>
                JsonSerializer.Deserialize<TMessage>(Encoding.UTF8.GetString(x.Data.Contents), _jsonSerializerOptions)
                ?? throw new Exception("failed to deserialize message"))
            .ToArray();
        
        var tasks = new Task[_handlers.Length];
        for (int i = 0; i < _handlers.Length; i++)
        {
            tasks[i] = _handlers[i].Handle(batch);
        }
        
        await Task.WhenAll(tasks);
        
        await _consumer.StoreOffset(offset);
    }
}