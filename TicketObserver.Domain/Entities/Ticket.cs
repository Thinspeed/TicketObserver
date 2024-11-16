using Generator.Attributes;
using TicketObserver.Domain.Abstractions;

namespace TicketObserver.Domain.Entities;

[EfConstructor]
public partial class Ticket : Entity
{
    [RelationId(RelationTypeName = nameof(Entities.Train))]
    private long _trainId;
    
    public DateTime ObservedTime { get; set; }
}