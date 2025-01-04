namespace TicketHandling.DebeziumConsumer.Consumers.Abstractions;

public interface ICustomConsumer
{
    bool IsRunning { get; } 
    Task StartAsync();
    
    Task StopAsync();
}