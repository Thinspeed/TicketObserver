using System;
using System.Linq.Expressions;

namespace testSolution.Selector;

public class Selector<TSource, TDestination>
{
    public Expression<Func<TSource, TDestination>> Expression { get; }

    internal Selector(MemberInitExpression memberInit, ParameterExpression parameter)
    {
        Expression = System.Linq.Expressions.Expression.Lambda<Func<TSource, TDestination>>(memberInit, parameter);
    }
}

