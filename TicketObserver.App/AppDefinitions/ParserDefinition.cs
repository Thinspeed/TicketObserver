using AppDefinition.Abstractions;
using EfSelector.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EfSelector.AppDefinitions;

public class ParserDefinition : IAppDefinition
{
    public void RegisterDefinition(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IParser, DefaultParser>();
    }
}