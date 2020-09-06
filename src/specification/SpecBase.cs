using System;
using System.Linq.Expressions;

namespace DesignPatterns.Specification
{
    public abstract class SpecBase<T> : ISpecification<T>
    {
        protected bool Equals(SpecBase<T> other) => Equals(_compiledFunc, other._compiledFunc);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SpecBase<T>) obj);
        }

        public override int GetHashCode() => (_compiledFunc != null ? _compiledFunc.GetHashCode() : 0);

        private Func<T, bool> _compiledFunc;

        public virtual bool IsSatisfiedBy(T candidate)
        {
            _compiledFunc ??= Expression.Compile();
            return _compiledFunc(candidate);
        }

        public abstract Expression<Func<T, bool>> Expression { get; }

        public static And<T> operator &(SpecBase<T> first, SpecBase<T> second) => new And<T>(first, second);

        public static Or<T> operator |(SpecBase<T> first, SpecBase<T> second) => new Or<T>(first, second);

        public static SpecBase<T> operator ==(bool value, SpecBase<T> spec) => value ? spec : !spec;

        public static SpecBase<T> operator ==(SpecBase<T> spec, bool value) => value ? spec : !spec;

        public static SpecBase<T> operator !=(bool value, SpecBase<T> spec) => value ? !spec : spec;

        public static SpecBase<T> operator !=(SpecBase<T> spec, bool value) => value ? !spec : spec;

        public static Not<T> operator !(SpecBase<T> spec) => new Not<T>(spec);

        public static implicit operator Expression<Func<T, bool>>(SpecBase<T> spec) => spec.Expression;

        public static implicit operator Func<T, bool>(SpecBase<T> spec) => spec.IsSatisfiedBy;

        public override string ToString() => Expression.ToString();

        public sealed class And<T> : SpecBase<T>, IAndSpecification<T>
        {
            public SpecBase<T> First { get; }

            public SpecBase<T> Second { get; }

            ISpecification<T> IAndSpecification<T>.First => First;

            ISpecification<T> IAndSpecification<T>.Second => Second;

            internal And(SpecBase<T> first, SpecBase<T> second)
            {
                First = first ?? throw new ArgumentNullException(nameof(first));
                Second = second ?? throw new ArgumentNullException(nameof(second));
            }

            public override Expression<Func<T, bool>> Expression => First.Expression.And(Second.Expression);

            public new bool IsSatisfiedBy(T candidate) => First.IsSatisfiedBy(candidate) && Second.IsSatisfiedBy(candidate);
        }

        public sealed class Or<T> : SpecBase<T>, IOrSpecification<T>
        {
            public SpecBase<T> First { get; }

            public SpecBase<T> Second { get; }

            ISpecification<T> IOrSpecification<T>.First => First;

            ISpecification<T> IOrSpecification<T>.Second => Second;

            internal Or(SpecBase<T> first, SpecBase<T> second)
            {
                First = first ?? throw new ArgumentNullException(nameof(first));
                Second = second ?? throw new ArgumentNullException(nameof(second));
            }

            public override Expression<Func<T, bool>> Expression => First.Expression.Or(Second.Expression);

            public bool Is(T candidate) => First.IsSatisfiedBy(candidate) || Second.IsSatisfiedBy(candidate);
        }

        public sealed class Not<T> : SpecBase<T>, INotSpecification<T>
        {
            public SpecBase<T> Inner { get; }

            ISpecification<T> INotSpecification<T>.Inner => Inner;

            internal Not(SpecBase<T> spec) => Inner = spec ?? throw new ArgumentNullException(nameof(spec));

            public override Expression<Func<T, bool>> Expression => Inner.Expression.Not();

            public bool Is(T candidate) => !Inner.IsSatisfiedBy(candidate);
        }
    }
}