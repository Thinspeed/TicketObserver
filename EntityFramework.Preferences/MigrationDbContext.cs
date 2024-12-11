using EntityFramework.Preferences.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Preferences;

public class MigrationDbContext : DbContext
{

    public MigrationDbContext(DbContextOptions<MigrationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigurationMarker).Assembly);
    }
}