using AppDefinition.Abstractions;

namespace AppDefinition.Providers;

public class AppDefinitionProvider(HashSet<IAppDefinition> definitions) : IAppDefinitionProvider
{
    private readonly HashSet<IAppDefinition> _appDefinitions = definitions;
    
    public IEnumerable<IAppDefinition> GetAppDefinitions()
    {
        return _appDefinitions.ToList();
    }
}