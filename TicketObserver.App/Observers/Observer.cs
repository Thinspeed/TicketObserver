using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using EfSelector.Parsers;
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
    private readonly IParser _parser;
    
    public Observer(
        ILogger<Program> logger, 
        IConfiguration configuration, 
        ApplicationDbContext dbContext,
        IParser parser)
    {
        _workingThread = new Thread(Observe);

        _uri = configuration.GetSection("uri").Value ?? throw new NullReferenceException("uri");

        AngleSharp.IConfiguration angleConfig = Configuration.Default.WithDefaultLoader();
        _context = BrowsingContext.New(angleConfig);
        
        _dbContext = dbContext;
        _logger = logger;
        _parser = parser;
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
            
            List<Train> parser = _parser.GetAvailableTrains(document);
            
    
            Thread.Sleep(2000);
        }
    }
}