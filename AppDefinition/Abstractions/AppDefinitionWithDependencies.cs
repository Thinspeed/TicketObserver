using Microsoft.Extensions.DependencyInjection;

namespace AppDefinition.Abstractions;

public abstract class AppDefinitionWithDependencies : IAppDefinition
{
    private bool _isInitialized = false;
    
    protected virtual IAppDefinition[] DependsOn => [];
    
    public void Init(IServiceCollection serviceCollection)
    {
        if (_isInitialized) return;
        
        foreach (var definition in DependsOn)
        {
            definition.Init(serviceCollection);
        }
        
        Initialize();
        _isInitialized = true;
    }
    
    protected abstract void Initialize();
}