using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using TicketHandling.DebeziumConsumer.RabbitMQ;
using TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;
using TicketHandling.DebeziumConsumer.RabbitMQ.Consumers;
using TicketHandling.DebeziumConsumer.RabbitMQ.Handlers;

var conf = new StreamConsumerFactoryConfiguration()
{
    UserName = "observer",
    Password = "1234",
    VirtualHost = "/observer",
    HostIp = "192.168.0.102",
    StreamName = "tickets_stream",
    Reference = "TicketHandling.DebeziumConsumer"
};
    
var consumerFactory = await StreamConsumerFactory<DebeziumMessage>.CreateAsync(conf);

ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("192.168.0.102:6379,password=redis");

ICustomConsumer consumer = consumerFactory.CreateConsumer([new DebeziumBatchHandler(redis)]);

await consumer.StartAsync();

Console.ReadKey();

await consumer.StopAsync();

redis.Dispose();


HostApplicationBuilder builder = Host.CreateApplicationBuilder();

IHost app = builder.Build();

app.Run();