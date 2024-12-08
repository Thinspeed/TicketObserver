using Microsoft.Extensions.DependencyInjection;

namespace AppDefinition.Abstractions;

public interface IAppDefinition
{
    void Init(IServiceCollection serviceCollection);
}