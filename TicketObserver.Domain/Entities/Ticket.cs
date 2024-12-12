using Generator.Attributes;
using TicketObserver.Domain.Abstractions;

namespace TicketObserver.Domain.Entities;

[EfConstructor]
public partial class Ticket : Entity
{
    // [RelationId(RelationTypeName = nameof(Entities.Train))]
    // private long _trainId;

    public Ticket(string trainNumber, DateTime departureDate, DateTime observedTime)
    {
        TrainNumber = trainNumber;
        DepartureDate = departureDate;
        ObservedTime = observedTime;
    }
    
    public string TrainNumber { get; set; }
    
    public DateTime DepartureDate { get; set; }
    
    public DateTime ObservedTime { get; set; }
}