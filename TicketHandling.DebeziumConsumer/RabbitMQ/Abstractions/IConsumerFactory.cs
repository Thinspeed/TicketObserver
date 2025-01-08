namespace TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;

public interface IConsumerFactory;

public interface IConsumerFactory<out TConsumer, TMessage> : IConsumerFactory
    where TConsumer : ICustomConsumer
{
    public TConsumer CreateConsumer(IMessageHandler<TMessage>[] handlers);
}