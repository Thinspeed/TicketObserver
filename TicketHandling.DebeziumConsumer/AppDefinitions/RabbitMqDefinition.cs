using AppDefinition.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicketHandling.DebeziumConsumer.RabbitMQ.Abstractions;
using TicketHandling.DebeziumConsumer.RabbitMQ.Consumers;
using TicketHandling.DebeziumConsumer.RabbitMQ.Models;

namespace TicketHandling.DebeziumConsumer.AppDefinitions;

public class RabbitMqDefinition : IAppDefinition
{
    public void RegisterDefinition(IHostApplicationBuilder builder)
    {
        StreamConsumerFactoryConfiguration configuration =
            builder.Configuration.GetSection("RabbitMq").Get<StreamConsumerFactoryConfiguration>()
            ?? throw new Exception($"RabbitMq configuration was not provided");

        IConsumerFactory<ICustomConsumer, DebeziumMessage[]> factory = StreamBatchConsumerFactory<DebeziumMessage>
            .CreateAsync(configuration)
            .GetAwaiter()
            .GetResult();

        builder.Services.AddKeyedSingleton<IConsumerFactory<ICustomConsumer, DebeziumMessage[]>>(nameof(DebeziumMessage), factory);
    }
}