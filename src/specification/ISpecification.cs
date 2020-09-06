using System;
using System.Linq.Expressions;

namespace DesignPatterns.Specification
{
    public interface ISpecification
    {
    }

    public interface ISpecification<T> : ISpecification
    {
        bool IsSatisfiedBy(T candidate);
        Expression<Func<T, bool>> Expression { get; }
    }

    public interface IAndSpecification<T> : ISpecification<T>
    {
        ISpecification<T> First { get; }
        ISpecification<T> Second { get; }
    }

    public interface IOrSpecification<T> : ISpecification<T>
    {
        ISpecification<T> First { get; }
        ISpecification<T> Second { get; }
    }

    public interface INotSpecification<T> : ISpecification<T>
    {
        ISpecification<T> Inner { get; }
    }
}