using Generator.Attributes;
using TicketObserver.Domain.Abstractions;

namespace TicketObserver.Domain.Entities;

[EfConstructor]
public partial class Train : Entity
{
    public DateTime StartDate { get; set; }
}