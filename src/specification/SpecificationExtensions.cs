using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.Specification
{
    public static class SpecificationExtensions
    {
        public static bool Is<T>(this T candidate, ISpecification<T> spec) => spec.IsSatisfiedBy(candidate);

        public static bool Are<T>(this IEnumerable<T> candidates, ISpecification<T> spec) =>
            candidates.All(spec.IsSatisfiedBy);

        public static IAndSpecification<T> And<T>(this ISpecification<T> first, ISpecification<T> second) =>
            new AndSpecification<T>(first, second);

        public static IOrSpecification<T> Or<T>(this ISpecification<T> first, ISpecification<T> second) =>
            new OrSpecification<T>(first, second);

        public static INotSpecification<T> Not<T>(this ISpecification<T> inner) => new NotSpecification<T>(inner);
    }
}