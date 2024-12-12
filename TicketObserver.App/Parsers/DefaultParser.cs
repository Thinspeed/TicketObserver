using System.Globalization;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using TicketObserver.Domain.Entities;

namespace EfSelector.Parsers;

public class DefaultParser : IParser
{
    private const string Attribute = "data-train-number";
    private const string CellWithNumberClass = "cell-1";
    private const string CellWithTimeClass = "cell-4";
    private const string EmptyCellClass = "empty";

    private readonly ILogger _logger;
    
    public DefaultParser(ILogger<Program> logger)
    {
        _logger = logger;
    }
    
    public List<Ticket> GetAvailableTrains(IDocument document)
    {
        List<IElement> rows = document.All.Where(row => row.HasAttribute(Attribute)).ToList();
        List<Ticket> trains = [];
        
        foreach (IElement row in rows)
        {
            IElement? cellWithTime = row.Children.LastOrDefault();
            IElement? cellWithNumber = row.Children.FirstOrDefault();
                
            if (cellWithTime is null || !cellWithTime.ClassList.Contains(CellWithTimeClass) ||
                cellWithNumber is null || cellWithNumber.ClassList.Contains(CellWithNumberClass))
            {
                _logger.LogCritical(row.TextContent);
                    
                continue;
            }
        
            //если нет мест то на аттрибут навешивается дополнительный класс empty
            if (cellWithTime.ClassList.Contains(EmptyCellClass))
            {
                continue;
            }

            DateTime time = DateTime.ParseExact(
                row.QuerySelector(".train-from-time")!.InnerHtml, "HH:mm",
                CultureInfo.InvariantCulture);
                
            string trainNumber = row.QuerySelector(".train-number")!.InnerHtml;
            
            var train = new Ticket(trainNumber, time, DateTime.Now);
            trains.Add(train);
            //_logger.LogInformation($"Доступен билет на поезд в {time.Hour}:{time.Minute}");
        }

        return trains;
    }
}