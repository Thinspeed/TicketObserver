namespace TicketHandling.DebeziumConsumer;

public class ChangeOf<T> 
    where T : class
{
    public T? Before { get; set; }
    
    public T? After { get; set; }
}