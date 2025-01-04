
namespace TicketHandling.DebeziumConsumer.Consumers;

public class TicketTO
{
    public long Id { get; set; }
    
    public DateTime ObservedTime { get; set; }
    
    public DateTime DepartureDate { get; set; }
    
    public string TrainNumber { get; set; }
}