using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DesignPatterns.Specification
{
    public abstract class SqlExpressionVisitor
    {
        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression) exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression) exp);
                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression) exp);
                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression) exp);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression) exp);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression) exp);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression) exp);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression) exp);
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression) exp);
                case ExpressionType.New:
                    return VisitNew((NewExpression) exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression) exp);
                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression) exp);
                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression) exp);
                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression) exp);
                default:
                    throw new Exception($"Unhandled expression type: '{exp.NodeType}'");
            }
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment) binding);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding) binding);
                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding) binding);
                default:
                    throw new Exception($"Unhandled binding type '{binding.BindingType}'");
            }
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.Arguments);

            return arguments != initializer.Arguments
                ? Expression.ElementInit(initializer.AddMethod, arguments)
                : initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            var operand = Visit(u.Operand);

            return operand != u.Operand ? Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method) : u;
        }

        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            var left = Visit(b.Left);
            var right = Visit(b.Right);
            var conversion = Visit(b.Conversion);

            if (left == b.Left && right == b.Right && conversion == b.Conversion) return b;

            if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                return Expression.Coalesce(left, right, conversion as LambdaExpression);

            return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            var expr = Visit(b.Expression);

            return expr != b.Expression ? Expression.TypeIs(expr, b.TypeOperand) : b;
        }

        protected virtual Expression VisitConstant(ConstantExpression c) => c;

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            var test = Visit(c.Test);
            var ifTrue = Visit(c.IfTrue);
            var ifFalse = Visit(c.IfFalse);

            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
                return Expression.Condition(test, ifTrue, ifFalse);

            return c;
        }

        protected virtual Expression VisitParameter(ParameterExpression p) => p;

        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            var exp = Visit(m.Expression);

            return exp != m.Expression ? Expression.MakeMemberAccess(exp, m.Member) : m;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            var obj = Visit(m.Object);

            IEnumerable<Expression> args = VisitExpressionList(m.Arguments);

            if (obj != m.Object || !Equals(args, m.Arguments))
                return Expression.Call(obj, m.Method, args);

            return m;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = Visit(original[i]);

                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);

                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(p);
                }
            }

            return list != null ? new ReadOnlyCollection<Expression>(list) : original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var e = Visit(assignment.Expression);

            return e != assignment.Expression ? Expression.Bind(assignment.Member, e) : assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.Bindings);

            return !Equals(bindings, binding.Bindings) ? Expression.MemberBind(binding.Member, bindings) : binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.Initializers);

            return !Equals(initializers, binding.Initializers)
                ? Expression.ListBind(binding.Member, initializers)
                : binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                var b = VisitBinding(original[i]);

                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);

                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(b);
                }
            }

            if (list != null)
                return list;

            return original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                var init = VisitElementInitializer(original[i]);

                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);

                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(init);
                }
            }

            if (list != null)
                return list;

            return original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambdaExp)
        {
            var body = Visit(lambdaExp.Body);

            return body != lambdaExp.Body ? Expression.Lambda(lambdaExp.Type, body, lambdaExp.Parameters) : lambdaExp;
        }

        protected virtual NewExpression VisitNew(NewExpression newExp)
        {
            var args = VisitExpressionList(newExp.Arguments);

            if (!Equals(args, newExp.Arguments))
                return newExp.Members != null
                    ? Expression.New(newExp.Constructor, args, newExp.Members)
                    : Expression.New(newExp.Constructor, args);

            return newExp;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression initExp)
        {
            var n = VisitNew(initExp.NewExpression);
            var bindings = VisitBindingList(initExp.Bindings);

            if (n != initExp.NewExpression || !Equals(bindings, initExp.Bindings))
                return Expression.MemberInit(n, bindings);

            return initExp;
        }

        protected virtual Expression VisitListInit(ListInitExpression initExp)
        {
            var n = VisitNew(initExp.NewExpression);
            var initializers = VisitElementInitializerList(initExp.Initializers);

            if (n != initExp.NewExpression || !Equals(initializers, initExp.Initializers))
                return Expression.ListInit(n, initializers);

            return initExp;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression newExp)
        {
            var list = VisitExpressionList(newExp.Expressions);

            if (!Equals(list, newExp.Expressions))
                return newExp.NodeType == ExpressionType.NewArrayInit
                    ? Expression.NewArrayInit(newExp.Type.GetElementType(), list)
                    : Expression.NewArrayBounds(newExp.Type.GetElementType(), list);

            return newExp;
        }

        protected virtual Expression VisitInvocation(InvocationExpression exp)
        {
            var args = VisitExpressionList(exp.Arguments);
            var expr = Visit(exp.Expression);

            if (!Equals(args, exp.Arguments) || expr != exp.Expression)
                return Expression.Invoke(expr, args);

            return exp;
        }
    }
}