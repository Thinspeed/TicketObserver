﻿using System.Reflection;
using EntityFramework.Preferences.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Preferences;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigurationMarker).Assembly);
    }
}