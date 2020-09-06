using System;
using System.Linq.Expressions;

namespace DesignPatterns.Specification
{
    public static class Spec
    {
        public static SpecBase<T> Any<T>() => Spec<T>.Any;

        public static SpecBase<T> Not<T>() => Spec<T>.None;
    }

    public class Spec<T> : SpecBase<T>
    {
        private readonly Expression<Func<T, bool>> _expression;

        public static readonly SpecBase<T> Any = new Spec<T>(x => true);

        public static readonly SpecBase<T> None = new Spec<T>(x => false);

        private readonly Lazy<Func<T, bool>> _compiledExpression;

        public Spec(Expression<Func<T, bool>> expression)
        {
            _expression = expression;
            _compiledExpression = new Lazy<Func<T, bool>>(() => _expression.Compile());
        }

        public override Expression<Func<T, bool>> Expression => _expression;

        public override bool IsSatisfiedBy(T candidate) => _compiledExpression.Value(candidate);
    }
}