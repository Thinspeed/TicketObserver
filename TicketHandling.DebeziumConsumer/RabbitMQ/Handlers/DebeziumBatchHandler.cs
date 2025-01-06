using System.Text.Json;
using StackExchange.Redis;
using TicketHandling.DebeziumConsumer.Consumers;
using TicketHandling.DebeziumConsumer.Extensions;
using TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;
using TicketObserver.Domain.Entities;

namespace TicketHandling.DebeziumConsumer.RabbitMQ.Handlers;

public class DebeziumBatchHandler : IMessageHandler<DebeziumMessage[]>
{
    private ConnectionMultiplexer _redis;
    private IDatabase _db;
    
    public DebeziumBatchHandler(ConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }
    
    public async Task Handle(DebeziumMessage[] batch)
    {
        var tickets = batch
            .Where(x => x.After is not null && x.Source.Table == nameof(Ticket))
            .Select(x => x.AsChangeOf<TicketTO>().After!)
            .GroupBy(x => x.DepartureDate.Date.Ticks);

        foreach (var group in tickets)
        {
            RedisValue redisValue = await _db.StringGetAsync(group.Key.ToString());
            List<TicketTO> redisTickets = null!;

            if (redisValue.HasValue)
                redisTickets = JsonSerializer.Deserialize<List<TicketTO>>(redisValue);
            
            redisTickets ??= new List<TicketTO>();

            IEnumerable<TicketTO> newTickets = group
                .Where(nt => redisTickets.All(rt => nt.Id != rt.Id));

            foreach (var ticket in newTickets)
            {
                Console.WriteLine($"Новый билет: {ticket.DepartureDate} {ticket.TrainNumber}");
            }

            string json = JsonSerializer.Serialize<List<TicketTO>>(group.ToList());
            await _db.StringSetAsync(group.Key.ToString(), json, TimeSpan.FromSeconds(30));
        }
    
        await Task.CompletedTask;
    }
}