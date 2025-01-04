namespace TicketHandling.DebeziumConsumer.Consumers.Abstractions;

public interface IConsumerFactory<out TConsumer, TMessage>
    where TConsumer : ICustomConsumer
{
    public TConsumer CreateConsumer(Func<TMessage[], Task> messageHandler);
}