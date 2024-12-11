using AppDefinition.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EfSelector.AppDefinitions;

public class LoggerDefinition : IAppDefinition
{
    public void RegisterDefinition(IHostApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;

        var loggerFactory = LoggerFactory.Create(loggerBuilder => loggerBuilder
            .AddConfiguration(configuration.GetSection("Logging"))
            .AddConsole());
        
        builder.Services.AddSingleton<ILoggerFactory>(loggerFactory);
        builder.Services.AddTransient(typeof(ILogger<>), typeof(Logger<>));
    }
}