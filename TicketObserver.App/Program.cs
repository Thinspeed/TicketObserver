using AppDefinition.Extensions;
using EfSelector;
using EfSelector.Observer;
using EntityFramework.Preferences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TicketObserver.Domain.Entities;


HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Logging.AddConsole();

builder.Services.AddHostedService<ObserverBackgroundService>();
builder.Services.AddSingleton<ITicketObserver, Observer>();

builder.AddAppDefinitions();

IHost app = builder.Build();

app.InitAppDefinitions();

ApplicationDbContext? dbContext = app.Services.GetService<ApplicationDbContext>();
var a = dbContext!.Set<Ticket>().ToList();

await app.RunAsync();