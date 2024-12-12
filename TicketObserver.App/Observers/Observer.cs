using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using EntityFramework.Preferences;
using Microsoft.Extensions.Logging;
using TicketObserver.Domain.Entities;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace EfSelector.Observer;

public class Observer : ITicketObserver
{
    private string _uri;
    private long _isRunning = 0;
    private readonly Thread _workingThread;

    private readonly IBrowsingContext _context;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger _logger;
    
    
    public Observer(ILogger<Program> logger, IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _workingThread = new Thread(Observe);

        _uri = configuration.GetSection("uri").Value ?? throw new NullReferenceException("uri");

        AngleSharp.IConfiguration angleConfig = Configuration.Default.WithDefaultLoader();
        _context = BrowsingContext.New(angleConfig);
        
        _dbContext = dbContext;
        
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
            _logger.LogInformation($"{DateTime.Now}: sending request...");
            
            IDocument document = await _context.OpenAsync(_uri);
            
            string attribute = "data-train-number";
            string cellWithNumberClass = "cell-1";
            string cellWithTimeClass = "cell-4";
            string emptyCellClass = "empty";
            
            List<IElement> rows = document.All.Where(row => row.HasAttribute(attribute)).ToList();
            
            foreach (IElement row in rows)
            {
                IElement? cellWithTime = row.Children.LastOrDefault();
                IElement? cellWithNumber = row.Children.FirstOrDefault();
                
                if (cellWithTime is null || !cellWithTime.ClassList.Contains(cellWithTimeClass) ||
                    cellWithNumber is null || cellWithNumber.ClassList.Contains(cellWithNumberClass))
                {
                    _logger.LogCritical(row.TextContent);
                    
                    continue;
                }
        
                if (cellWithTime.ClassList.Contains(emptyCellClass))
                {
                    continue;
                }

                DateTime time = DateTime.ParseExact(row.QuerySelector(".train-from-time")!.InnerHtml, "HH:mm", CultureInfo.InvariantCulture);
                if (time.Hour < 6 || time.Hour > 15)
                {
                    continue;
                }
                
                string trainNumber = row.QuerySelector(".train-number")!.InnerHtml;
                var train = new Train(trainNumber, time);

                //_logger.LogInformation($"Доступен билет на поезд в {time.Hour}:{time.Minute}");
            }
    
            Thread.Sleep(2000);
        }
    }
}