using System;
using System.Linq.Expressions;

namespace DesignPatterns.Specification
{
    internal class OrSpecification<TEntity> : IOrSpecification<TEntity>
    {
        public ISpecification<TEntity> First { get; }

        public ISpecification<TEntity> Second { get; }

        internal OrSpecification(ISpecification<TEntity> first, ISpecification<TEntity> second)
        {
            First = first ?? throw new ArgumentNullException(nameof(first));
            Second = second ?? throw new ArgumentNullException(nameof(second));
        }

        public Expression<Func<TEntity, bool>> Expression => First.Expression.Or(Second.Expression);

        public bool IsSatisfiedBy(TEntity candidate) => First.IsSatisfiedBy(candidate) || Second.IsSatisfiedBy(candidate);
    }
}