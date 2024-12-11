using Generator.Attributes;
using TicketObserver.Domain.Abstractions;

namespace TicketObserver.Domain.Entities;

[EfConstructor]
public partial class Train : Entity
{
    public string Number { get; set; }
    
    public DateTime StartDate { get; set; }
}