namespace TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;

public interface ICustomConsumer
{
    bool IsRunning { get; } 
    Task StartAsync();
    Task StopAsync();
}