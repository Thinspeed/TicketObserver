namespace EfSelector.Parsers.TicketParser;

public class TicketParserModel
{
    public TicketParserModel(string trainNumber, TimeOnly departureTime)
    {
        TrainNumber = trainNumber;
        DepartureTime = departureTime;
    }
    
    public string TrainNumber { get; set; }
    
    public TimeOnly DepartureTime { get; set; }
}