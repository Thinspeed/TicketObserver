using System.Net;
using RabbitMQ.Stream.Client;
using TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;

namespace TicketHandling.DebeziumConsumer.RabbitMQ.Consumers;

public class StreamConsumerFactoryConfiguration
{
    public required string UserName { get; init; }

    public required string Password { get; init; }

    public required string VirtualHost { get; init; }

    public required string HostIp { get; init; }

    public int Port { get; init; } = 5552;
    
    public required string StreamName { get; init; }
    
    public required string Reference { get; init; }
}

//todo добавить деструктор, либо сделать IConsumerFactory disposable
//todo тоже для ICustomConsumer, нужна очистка ресурсов
public class StreamBatchConsumerFactory<TMessage> : IConsumerFactory<BatchStreamConsumer<TMessage>, TMessage[]>
{
    private StreamSystem _streamSystem;
    private string _streamName;
    private string _reference;
    
    private StreamBatchConsumerFactory(StreamSystem streamSystem, string streamName, string reference)
    {
        _streamSystem = streamSystem;
        _streamName = streamName;
        _reference = reference;
    } 
    
    public static async Task<StreamBatchConsumerFactory<TMessage>> CreateAsync(StreamConsumerFactoryConfiguration configuration)
    {
        var streamSystemConfig = new StreamSystemConfig()
        {
            UserName = configuration.UserName,
            Password = configuration.Password,
            VirtualHost = configuration.VirtualHost,
            Endpoints = [new IPEndPoint(IPAddress.Parse(configuration.HostIp), configuration.Port)],
        };

        var streamSystem = await StreamSystem.Create(streamSystemConfig);

        var factory = new StreamBatchConsumerFactory<TMessage>(streamSystem, configuration.StreamName, configuration.Reference);
        
        return factory;
    }
    
    
    public BatchStreamConsumer<TMessage> CreateConsumer(IMessageHandler<TMessage[]>[] handlers)
    {
        var consumer = new BatchStreamConsumer<TMessage>(_streamSystem, _reference, _streamName, 10, 1500, handlers);
        
        return consumer;
    }
}