using AppDefinition.Abstractions;
using EntityFramework.Preferences;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EfSelector.AppDefinitions;

public class EntityFrameworkDefinition : IAppDefinition
{
    public void AddDefinition(IHostApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetSection("ReadWriteConnectionString").Value 
                                   ?? throw new InvalidOperationException("Read/write connection string was not provided");
        
        string migrationConnectionString = builder.Configuration.GetSection("MigrationConnectionString").Value 
                                            ?? throw new InvalidOperationException("Read/write connection string was not provided");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(connectionString));
        
        builder.Services.AddDbContext<MigrationDbContext>(options =>
            options.UseNpgsql(migrationConnectionString));
    }

    public void Init(IServiceProvider serviceProvider)
    {
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        
        MigrationDbContext dbContext = serviceProvider.GetService<MigrationDbContext>() 
                                       ?? throw new InvalidOperationException("Migration context was not provided");
        
        dbContext.Database.Migrate();
    }
}