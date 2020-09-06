using System.Collections.Generic;
using System.Linq.Expressions;

namespace DesignPatterns.Specification
{
    public class ParameterRebinder : SqlExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map) =>
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
            Expression exp) =>
            new ParameterRebinder(map).Visit(exp);

        protected override Expression VisitParameter(ParameterExpression p) =>
            base.VisitParameter(_map.TryGetValue(p, out var replacement) ? replacement : p);
    }
}