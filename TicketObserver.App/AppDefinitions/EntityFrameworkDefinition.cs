using AppDefinition.Abstractions;
using EntityFramework.Preferences;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EfSelector.AppDefinitions;

public class EntityFrameworkDefinition : IAppDefinition
{
    public void RegisterDefinition(IHostApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetSection("ReadWriteConnectionString").Value 
                                   ?? throw new InvalidOperationException("Read/write connection string was not provided");
        
        string migrationConnectionString = builder.Configuration.GetSection("MigrationConnectionString").Value 
                                  ?? throw new InvalidOperationException("Migration connection string was not provided");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(connectionString));
        
        builder.Services.AddDbContext<MigrationDbContext>(options => options
            .UseNpgsql(migrationConnectionString));
    }

    public void Init(IServiceProvider serviceProvider)
    {
        ILogger<Program> logger = serviceProvider.GetService<ILogger<Program>>()!;
        
        using var dbContext = serviceProvider.GetRequiredService<MigrationDbContext>();
        
        logger.LogInformation("Применение миграций...");
        
        dbContext.Database.Migrate();
        
        logger.LogInformation("Миграции применены."); 
    }
}