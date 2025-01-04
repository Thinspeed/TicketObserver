namespace TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;

public interface IMessageHandler<T>
{
    public Task Handle(T message);
}