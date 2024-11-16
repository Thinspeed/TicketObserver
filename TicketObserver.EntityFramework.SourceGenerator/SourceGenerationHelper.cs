using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TicketObserver.EntityFramework.SourceGenerator;

public static class SourceGenerationHelper
{
    public const string AttributesNamespace = "Generator.Attributes";
    public static string GetGeneratedCodeAttribute(string source, string version) => 
        $"[global::System.CodeDom.Compiler.GeneratedCode(\"{source}\", \"{version}\")]";
    
    public static bool IsSyntaxTargetClassWithAttribute(SyntaxNode syntaxNode, string attributeName)
    {
        return syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
               classDeclarationSyntax.AttributeLists.Count > 0 &&
               classDeclarationSyntax.AttributeLists
                   .Any(al => al.Attributes
                       .Any(a => a.Name.ToString() == attributeName));
    }

    public static bool IsSyntaxTargetFieldWithAttribute(SyntaxNode syntaxNode, string attributeName)
    {
        return syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax &&
               fieldDeclarationSyntax.AttributeLists.Count > 0 &&
               fieldDeclarationSyntax.AttributeLists
                   .Any(al => al.Attributes
                       .Any(a => a.Name.ToString() == attributeName));
    }

    public static ClassDeclarationSyntax GetTargetClassForGeneration(GeneratorSyntaxContext  syntaxNode)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)syntaxNode.Node;
        
        return classDeclarationSyntax;
    }

    public static FieldDeclarationSyntax GetTargetFieldForGeneration(GeneratorSyntaxContext syntaxNode)
    {
        var fieldDeclarationSyntax = (FieldDeclarationSyntax)syntaxNode.Node;
        
        return fieldDeclarationSyntax;
    }
}