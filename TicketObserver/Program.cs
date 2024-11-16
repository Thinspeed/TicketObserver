using EfSelector;
using EfSelector.Observer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Logging.AddConsole();

builder.Services.AddHostedService<ObserverBackgroundService>();
builder.Services.AddSingleton<ITicketObserver, Observer>();

IHost app = builder.Build();

await app.RunAsync();