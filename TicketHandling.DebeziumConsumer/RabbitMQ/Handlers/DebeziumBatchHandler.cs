using TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;

namespace TicketHandling.DebeziumConsumer.RabbitMQ.Handlers;

public class DebeziumBatchHandler : IMessageHandler<DebeziumMessage[]>
{
    public async Task Handle(DebeziumMessage[] batch)
    {
        Console.WriteLine($"Consumed messages: {batch.Length}");
        for (int i = 0; i < batch.Length; i++)
        {
            Console.WriteLine(batch[i]);
        }
    
        await Task.CompletedTask;
    }
}