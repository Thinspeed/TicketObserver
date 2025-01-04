using System.Text.Json;
using TicketHandling.DebeziumConsumer.Consumers;
using TicketHandling.DebeziumConsumer.RabbitMQ;

namespace TicketHandling.DebeziumConsumer.Extensions;

public static class DebeziumMessageExtension
{
    public static ChangeOf<T> AsChangeOf<T>(this DebeziumMessage message)
        where T : class
    {
        T? before = null, after = null;
        
        if (message.Before is JsonElement beforeElement)
        {
            before = JsonSerializer.Deserialize<T>(beforeElement.GetRawText());
        }

        if (message.After is JsonElement afterElement)
        {
            after = JsonSerializer.Deserialize<T>(afterElement.GetRawText());
        }

        var changeOf = new ChangeOf<T>()
        {
            Before = before,
            After = after
        };
        
        return changeOf;
    }
}