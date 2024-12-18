using AppDefinition.Abstractions;
using EfSelector.Parsers;
using EfSelector.Parsers.TicketParser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EfSelector.AppDefinitions;

public class ParserDefinition : IAppDefinition
{
    public void RegisterDefinition(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IParser<TicketParserModel>, TicketParser>();
    }
}