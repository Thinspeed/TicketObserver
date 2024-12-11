using System.Reflection;
using AppDefinition.Abstractions;
using AppDefinition.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppDefinition.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddAppDefinitions(this IHostApplicationBuilder builder)
    {
        var assembly = Assembly.GetCallingAssembly();
        
        IEnumerable<IAppDefinition> appDefinitions = assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(IAppDefinition).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as IAppDefinition)!;
        
        HashSet<IAppDefinition> handledDefinitions = [];

        foreach (var definition in appDefinitions)
        {
            builder.AddDefinition(definition, handledDefinitions);
        }

        var appDefinitionProvider = new AppDefinitionProvider(handledDefinitions);
        builder.Services.AddSingleton<IAppDefinitionProvider>(appDefinitionProvider);
    }

    private static void AddDefinition(this IHostApplicationBuilder builder, IAppDefinition definition, HashSet<IAppDefinition> handledDefinitions)
    {
        foreach (var dependency in definition.DependsOn)
        {
            if (!handledDefinitions.Contains(dependency))
            {
                AddDefinition(builder, dependency, handledDefinitions);
            }
        }
            
        definition.AddDefinition(builder);
    }
}