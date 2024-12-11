using EfSelector.Observer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EfSelector;

public class ObserverBackgroundService : BackgroundService
{
    //private ITicketObserver _ticketObserver;
    private IServiceScopeFactory _serviceScopeFactory;
    
    public ObserverBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        //_ticketObserver = ticketObserver;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        ITicketObserver? ticketObserver = scope.ServiceProvider.GetService<ITicketObserver>();
        
        if (ticketObserver == null)
            throw new ArgumentNullException(nameof(ticketObserver));
        
        ticketObserver.Start();
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
        
        ticketObserver.Stop();
    }
}