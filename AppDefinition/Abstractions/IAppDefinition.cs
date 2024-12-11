using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppDefinition.Abstractions;

public interface IAppDefinition
{
    public IAppDefinition[] DependsOn => [];
    
    void AddDefinition(IHostApplicationBuilder builder);

    void Init(IServiceProvider serviceProvider);
}