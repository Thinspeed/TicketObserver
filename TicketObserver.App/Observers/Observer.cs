using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using EfSelector.Parsers;
using EfSelector.Parsers.TicketParser;
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

    //todo нужно изменить логику(передавать дату или обновлять её при каждом запросе)
    private readonly DateOnly _observationDate;

    private readonly IBrowsingContext _browsingContext;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly IParser<TicketParserModel> _parser;

    public Observer(
        ILogger<Program> logger,
        IConfiguration configuration,
        ApplicationDbContext dbContext,
        IParser<TicketParserModel> parser)
    {
        _workingThread = new Thread(Observe);

        //_observationDate = DateOnly.FromDateTime(DateTime.Today);
        _observationDate = new DateOnly(2024, 12, 20);
        
        string uri = configuration.GetSection("uri").Value ?? throw new NullReferenceException("uri");
        _uri = uri + _observationDate.ToString("yyyy-MM-dd");

        AngleSharp.IConfiguration angleConfig = Configuration.Default.WithDefaultLoader();
        _browsingContext = BrowsingContext.New(angleConfig);

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
            if (_observationDate < DateOnly.FromDateTime(DateTime.Today))
            {
                _logger.LogCritical("End observation.");
                break;
            }

            _logger.LogInformation("Sending request...");

            IDocument document = await _browsingContext.OpenAsync(_uri);
            DateTime lastRequestTime = DateTime.Now;

            List<Ticket> availableTickets = _parser.Parse(document)
                .Select(x => new Ticket(
                    x.TrainNumber,
                    DateTime.SpecifyKind(_observationDate.ToDateTime(x.DepartureTime), DateTimeKind.Utc),
                    DateTime.SpecifyKind(lastRequestTime, DateTimeKind.Utc)))
                .ToList();

            foreach (var availableTicket in availableTickets)
            {
                var ticket = _dbContext.Set<Ticket>().FirstOrDefault(t => 
                    t.TrainNumber == availableTicket.TrainNumber && t.DepartureDate == availableTicket.DepartureDate);

                if (ticket is null)
                {
                    _dbContext.Set<Ticket>().Add(availableTicket);
                }
                else
                {
                    ticket.ObservedTime = availableTicket.ObservedTime;
                }
            }
            
            await _dbContext.SaveChangesAsync();
            
            Thread.Sleep(2000);
        }
    }
}