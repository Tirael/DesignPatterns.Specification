using System;
using System.Linq.Expressions;

namespace DesignPatterns.Specification
{
    internal class AndSpecification<T> : IAndSpecification<T>
    {
        public ISpecification<T> First { get; }

        public ISpecification<T> Second { get; }

        internal AndSpecification(ISpecification<T> first, ISpecification<T> second)
        {
            First = first ?? throw new ArgumentNullException(nameof(first));
            Second = second ?? throw new ArgumentNullException(nameof(second));
        }

        public Expression<Func<T, bool>> Expression => First.Expression.And(Second.Expression);

        public bool IsSatisfiedBy(T candidate) => First.IsSatisfiedBy(candidate) && Second.IsSatisfiedBy(candidate);
    }
}