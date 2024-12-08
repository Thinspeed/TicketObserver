using System.Reflection;
using AppDefinition.Abstractions;
using Microsoft.Extensions.Hosting;

namespace AppDefinition.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void InitAppDefinitions(this IHostApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        IEnumerable<IAppDefinition?> appDefinitions = assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(IAppDefinition).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as IAppDefinition);
        
        foreach (var definition in appDefinitions)
        {
            definition?.Init(builder.Services);
        }
    }
}