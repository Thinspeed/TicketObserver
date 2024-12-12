using AngleSharp.Dom;
using TicketObserver.Domain.Entities;

namespace EfSelector.Parsers;

public interface IParser
{
    List<Ticket> GetAvailableTrains(IDocument document);
}