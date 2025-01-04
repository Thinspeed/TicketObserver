using System.Net;
using RabbitMQ.Stream.Client;
using TicketHandling.DebeziumConsumer.Consumers.Abstractions;

namespace TicketHandling.DebeziumConsumer.Consumers.Defaults;

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

public class StreamConsumerFactory<TMessage> : IConsumerFactory<ICustomConsumer, TMessage>
{
    private StreamSystem _streamSystem;
    private string _streamName;
    private string _reference;
    
    private StreamConsumerFactory(StreamSystem streamSystem, string streamName, string reference)
    {
        _streamSystem = streamSystem;
        _streamName = streamName;
        _reference = reference;
    } 
    
    public static async Task<StreamConsumerFactory<TMessage>> CreateAsync(StreamConsumerFactoryConfiguration configuration)
    {
        var streamSystemConfig = new StreamSystemConfig()
        {
            UserName = configuration.UserName,
            Password = configuration.Password,
            VirtualHost = configuration.VirtualHost,
            Endpoints = [new IPEndPoint(IPAddress.Parse(configuration.HostIp), configuration.Port)],
        };

        var streamSystem = await StreamSystem.Create(streamSystemConfig);

        var factory = new StreamConsumerFactory<TMessage>(streamSystem, configuration.StreamName, configuration.Reference);
        
        return factory;
    }
    
    
    public ICustomConsumer CreateConsumer(Func<TMessage[], Task> messageHandler)
    {
        var consumer = new BatchStreamConsumer<TMessage>(_streamSystem, _reference, _streamName, 10, 1500, messageHandler);
        
        return consumer;
    }
}