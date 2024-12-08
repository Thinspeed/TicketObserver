using System.Linq.Expressions;

namespace testSolution.Selector.Visitors;

public class MemberVisitor : ExpressionVisitor
{
    private readonly Expression _source;
    private readonly MemberExpression _replacement;

    public MemberVisitor(Expression source, MemberExpression replacement)
    {
        _source = source;
        _replacement = replacement;
    }
    
    protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
        if (node.Expression is not MemberExpression memberExpression || memberExpression.Expression != _source)
        {
            return base.VisitMemberAssignment(node);
        }
        
        MemberAssignment assignment = Expression.Bind(
            node.Member, 
            Expression.MakeMemberAccess(_replacement, memberExpression.Member));
        
        return assignment;
    }
}