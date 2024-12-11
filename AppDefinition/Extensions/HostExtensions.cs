using AppDefinition.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppDefinition.Extensions;

public static class HostExtensions
{
    public static void InitAppDefinitions(this IHost app)
    {
        IAppDefinitionProvider provider = app.Services.GetService<IAppDefinitionProvider>()
                                          ?? throw new InvalidOperationException("App definition provider not found");

        IEnumerable<IAppDefinition> appDefinitions = provider.GetAppDefinitions();
        foreach (var definition in appDefinitions)
        {
            definition.Init(app.Services);
        }
    }
}