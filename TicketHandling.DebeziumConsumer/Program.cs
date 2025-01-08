using AppDefinition.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;
using TicketHandling.DebeziumConsumer.RabbitMQ.Handlers.Debezium;
using TicketHandling.DebeziumConsumer.RabbitMQ.Models;

// var conf = new StreamConsumerFactoryConfiguration()
// {
//     UserName = "observer",
//     Password = "1234",
//     VirtualHost = "/observer",
//     HostIp = "192.168.0.102",
//     StreamName = "tickets_stream",
//     Reference = "TicketHandling.DebeziumConsumer"
// };
//     
// var consumerFactory = await StreamBatchConsumerFactory<DebeziumMessage>.CreateAsync(conf);
//
// ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("192.168.0.102:6379,password=redis");
//
// ICustomConsumer consumer = consumerFactory.CreateConsumer([new DebeziumBatchHandler(redis)]);
//
// await consumer.StartAsync();
//
// Console.ReadKey();
//
// await consumer.StopAsync();
//
// redis.Dispose();


HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Configuration.AddJsonFile("appsettings.json");

builder.AddAppDefinitions();

IHost app = builder.Build();
app.InitAppDefinitions();


var factory = app.Services
    .GetKeyedService<IConsumerFactory<ICustomConsumer, DebeziumMessage[]>>(nameof(DebeziumMessage));

IServiceScope scope = app.Services.CreateScope();
ConnectionMultiplexer redis = scope.ServiceProvider.GetService<ConnectionMultiplexer>()!;

//todo этот этап тоже можно вынести в AppDifinition 
ICustomConsumer consumer = factory!.CreateConsumer([new DebeziumBatchHandler(redis)]);
await consumer.StartAsync();

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

await consumer.StopAsync();