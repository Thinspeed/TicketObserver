using EfSelector.Observer;
using Microsoft.Extensions.Hosting;

namespace EfSelector;

public class ObserverBackgroundService : BackgroundService
{
    private ITicketObserver _ticketObserver;
    
    public ObserverBackgroundService(ITicketObserver ticketObserver)
    {
        _ticketObserver = ticketObserver;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ticketObserver.Start();
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
        
        _ticketObserver.Stop();
    }
}