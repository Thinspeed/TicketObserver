using Generator.Attributes;
using TicketObserver.Domain.Abstractions;

namespace TicketObserver.Domain.Entities;

// [EfConstructor]
// public partial class Train : Entity
// {
//     public Train(string number, DateTime startDate)
//     {
//         Number = number ?? throw new ArgumentNullException(nameof(number));
//         StartDate = startDate;
//     }
//     
//     public string Number { get; set; }
//     
//     public DateTime StartDate { get; set; }
// }