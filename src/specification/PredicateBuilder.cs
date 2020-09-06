using System;
using System.Linq;
using System.Linq.Expressions;

namespace DesignPatterns.Specification
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() => param => true;

        public static Expression<Func<T, bool>> False<T>() => param => false;

        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) => predicate;

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second) =>
            first.Compose(second, Expression.AndAlso);

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second) =>
            first.Compose(second, Expression.OrElse);

        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(k => k.s, e => e.f);

            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }
}