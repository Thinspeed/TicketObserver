namespace AppDefinition.Abstractions;

public interface IAppDefinitionProvider
{
    IEnumerable<IAppDefinition> GetAppDefinitions();
}