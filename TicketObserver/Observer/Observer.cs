using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EfSelector.Observer;

public class Observer : ITicketObserver
{
    private string _uri;
    private long _isRunning = 0;
    private Thread _workingThread;

    private readonly IBrowsingContext _context;
    private readonly ILogger<Observer> _logger;
    
    
    public Observer(ILogger<Observer> logger, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _workingThread = new Thread(Observe);

        _uri = configuration.GetSection("uri").Value ?? throw new NullReferenceException("uri");

        AngleSharp.IConfiguration angleConfig = Configuration.Default.WithDefaultLoader();
        _context = BrowsingContext.New(angleConfig);
        
        _logger = logger;
    }
    
    public void Start()
    {
        
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 0)
        {
            _workingThread.Start();
            _logger.LogInformation("Цикл запущен.");
        }
        else
        {
            _logger.LogInformation("Цикл уже работает.");
        }
    }
    
    public void Stop()
    {
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 0)
        {
            _workingThread.Join();
            _logger.LogInformation("Цикл запущен.");
        }
        else
        {
            _logger.LogInformation("Цикл уже работает.");
        }
    }

    private async void Observe()
    {
        while (Interlocked.Read(ref _isRunning) == 1)
        {
            IDocument document = await _context.OpenAsync(_uri);
            string attribute = "data-train-number";
            string countCellClass = "cell-4";
            string emptyCellClass = "empty";
            List<IElement> rows = document.All.Where(row => row.HasAttribute(attribute)).ToList();
            foreach (IElement row in rows)
            {
                Console.WriteLine($"{DateTime.Now}: sending request...");
                IElement? cell = row.Children.LastOrDefault();
                if (cell is null || !cell.ClassList.Contains(countCellClass))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    _logger.LogInformation(row.TextContent);
                    Console.ForegroundColor = ConsoleColor.Gray;

                    continue;
                }
        
                if (cell.ClassList.Contains(emptyCellClass))
                {
                    continue;
                }

                DateTime time = DateTime.ParseExact(row.QuerySelector(".train-from-time")!.InnerHtml, "HH:mm", CultureInfo.InvariantCulture);
                if (time.Hour < 6 || time.Hour > 15)
                {
                    continue;
                }
        
                Console.ForegroundColor = ConsoleColor.Green;
                _logger.LogInformation($"Доступен билет на поезд в {time.Hour}:{time.Minute}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
    
            Thread.Sleep(2000);
        }
    }
}