using System.Linq.Expressions;

namespace testSolution.Selector.Visitors;

internal class ParameterVisitor : ExpressionVisitor
{
    private readonly Expression _source;
    private readonly Expression _replacement;
    
    public ParameterVisitor(Expression source, Expression replacement)
    {
        _source = source;
        _replacement = replacement;
    }
    
    public override Expression? Visit(Expression? node)
    {
        return node == _source ? _replacement : base.Visit(node);
    }
}