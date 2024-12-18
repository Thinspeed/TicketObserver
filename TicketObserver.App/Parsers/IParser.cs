using AngleSharp.Dom;
using TicketObserver.Domain.Entities;

namespace EfSelector.Parsers;

public interface IParser<T>
{
    List<T> Parse(IDocument document);
}