using AppDefinition.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace TicketHandling.DebeziumConsumer.AppDefinitions;

public class RedisDefinition : IAppDefinition
{
    public void RegisterDefinition(IHostApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetSection("RedisConnectionString").Value
            ?? throw new Exception("Redis connection string was not found");

        builder.Services.AddScoped<ConnectionMultiplexer>(services => 
            ConnectionMultiplexer.Connect(connectionString));
    }
}