namespace TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;

public interface IConsumerFactory<out TConsumer, TMessage>
    where TConsumer : ICustomConsumer
{
    public TConsumer CreateConsumer(IMessageHandler<TMessage>[] handlers);
}