using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using testSolution.Selector.Visitors;

namespace testSolution.Selector;

public class EfSelector<TSource, TDestination>
{

    private readonly List<MemberAssignment> _assignmentExpressions = [];
    private readonly Expression _memberExpression = Expression.Variable(typeof(TSource));

    internal EfSelector() { }
    
    public EfSelector<TSource, TDestination> Select<TValue>(
        Expression<Func<TSource, TValue>> srcExpression,
        Expression<Func<TDestination, TValue>> dstExpression)
    {
        MemberExpression srcProperty = srcExpression.Body as MemberExpression 
                                       ?? throw new InvalidCastException($"{nameof(dstExpression)} body is not a MemberExpression");
        
        MemberExpression dstProperty = dstExpression.Body as MemberExpression 
                                       ?? throw new InvalidCastException($"{nameof(dstExpression)} body is not a MemberExpression");
        
        
        MemberAssignment assignment = Expression.Bind(
            dstProperty.Member,
            Expression.MakeMemberAccess(_memberExpression, srcProperty.Member));
        
        _assignmentExpressions.Add(assignment);

        return this;
    }

    public EfSelector<TSource, TDestination> Select<TSourceValue, TDestinationValue>(
        Expression<Func<TSource, TSourceValue>> srcExpression,
        Expression<Func<TDestination, TDestinationValue>> dstExpression,
        EfSelector<TSourceValue, TDestinationValue> selector)
    {
        MemberExpression srcProperty = srcExpression.Body as MemberExpression 
                                       ?? throw new InvalidCastException($"{nameof(dstExpression)} body is not a MemberExpression");
        
        MemberExpression dstProperty = dstExpression.Body as MemberExpression 
                                       ?? throw new InvalidCastException($"{nameof(dstExpression)} body is not a MemberExpression");

        MemberExpression memberAccess = Expression.MakeMemberAccess(_memberExpression, srcProperty.Member);
        var visitor = new MemberVisitor(selector._memberExpression, memberAccess);
        
        MemberInitExpression initExpression = selector.CreateInitExpression(visitor);
        MemberAssignment assignment = Expression.Bind(
            dstProperty.Member,
            initExpression);
        
        _assignmentExpressions.Add(assignment);
        
        return this;
    }

    public EfSelector<TSource, TDestination> SelectOrNull<TSourceValue, TDestinationValue>(
        Expression<Func<TSource, TSourceValue>> srcExpression,
        Expression<Func<TDestination, TDestinationValue>> dstExpression,
        EfSelector<TSourceValue, TDestinationValue> selector)
    {
        MemberExpression srcProperty = srcExpression.Body as MemberExpression 
                                       ?? throw new InvalidCastException($"{nameof(dstExpression)} body is not a MemberExpression");
        
        MemberExpression dstProperty = dstExpression.Body as MemberExpression 
                                       ?? throw new InvalidCastException($"{nameof(dstExpression)} body is not a MemberExpression");

        MemberExpression memberAccess = Expression.MakeMemberAccess(_memberExpression, srcProperty.Member);
        var visitor = new MemberVisitor(selector._memberExpression, memberAccess);
        
        MemberInitExpression initExpression = selector.CreateInitExpression(visitor);
        
        ConditionalExpression conditionalExpression = Expression.Condition(
            Expression.Equal(memberAccess, Expression.Constant(null)),
            Expression.Constant(null, typeof(TDestinationValue)),
            initExpression);
        
        MemberAssignment assignment = Expression.Bind(
            dstProperty.Member,
            conditionalExpression);
        
        _assignmentExpressions.Add(assignment);
        
        return this;
    }
    
    public Selector<TSource, TDestination> Construct()
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TSource));
        var visitor = new ParameterVisitor(_memberExpression, parameterExpression);
        
        MemberInitExpression expression = CreateInitExpression(visitor);
        var selector = new Selector<TSource, TDestination>(expression, parameterExpression);
        
        return selector;
    }

    private MemberInitExpression CreateInitExpression(ExpressionVisitor visitor)
    {
        NewExpression newExpression = Expression.New(typeof(TDestination));
        MemberInitExpression memberInit = Expression.MemberInit(newExpression, _assignmentExpressions);
        
        MemberInitExpression replacedTree = 
            visitor.Visit(memberInit) as MemberInitExpression
            ?? throw new NullReferenceException();

        return replacedTree;
    }
}

public static class EfSelector
{
    public static EfSelector<TSource, TDestination> Declare<TSource, TDestination>()
    {
        var selector = new EfSelector<TSource, TDestination>();
        
        return selector;
    }
}