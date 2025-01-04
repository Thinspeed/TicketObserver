namespace TicketHandling.DebeziumConsumer.RabbitMQ;

public class DebeziumMessage
{
    public object Before { get; set; }
    
    public object After { get; set; }

    public Source Source { get; set; }
    
    public string Op { get; set; }
    
    public long Ts_ms { get; set; }
    
    public long Ts_us { get; set; }
        
    public long Ts_ns { get; set; }
}

public struct Source
{
    public string Version { get; set; }
        
    public string Connector { get; set; }
        
    public string Name { get; set; }
        
    public long Ts_ms { get; set; }
        
    public string Snapshot { get; set; }
        
    public string Db { get; set; }
        
    public string Sequence { get; set; }

    public long Ts_us { get; set; }
        
    public long Ts_ns { get; set; }
        
    public string Schema { get; set; }
        
    public string Table { get; set; }
        
    public long TxId { get; set; }
        
    public long Lsn { get; set; }
        
    public long? Xmin { get; set; }
}