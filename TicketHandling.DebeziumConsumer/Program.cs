using System.Net;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using TicketHandling.DebeziumConsumer.Consumers;
using TicketHandling.DebeziumConsumer.Consumers.Abstractions;
using TicketHandling.DebeziumConsumer.Consumers.Defaults;

var conf = new StreamConsumerFactoryConfiguration()
{
    UserName = "observer",
    Password = "1234",
    VirtualHost = "/observer",
    HostIp = "192.168.0.102",
    StreamName = "tickets_stream",
    Reference = "TicketHandling.DebeziumConsumer"
};
    
IConsumerFactory<ICustomConsumer, DebeziumMessage> consumerFactory = await StreamConsumerFactory<DebeziumMessage>.CreateAsync(conf);
ICustomConsumer consumer = consumerFactory.CreateConsumer(async (DebeziumMessage[] batch) =>
{
    Console.WriteLine($"Consumed messages: {batch.Length}");
    for (int i = 0; i < batch.Length; i++)
    {
        Console.WriteLine(batch[i]);
    }
    
    await Task.CompletedTask;
});

await consumer.StartAsync();

Console.ReadKey();

await consumer.StopAsync();

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

IHost app = builder.Build();

app.Run();