using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HotLib
{
    public static class ExpressionHelpers
    {
        private static MethodInfo ReferenceEqualsMethod { get; } = ReflectionHelpers.CaptureMethod<object?, object?, bool>(ReferenceEquals);

        public static Expression IsNull(Expression expr)
        {
            if (Nullable.GetUnderlyingType(expr.Type) != null) // Need special handling for nullables
            {
                var hasValueProperty = expr.Type.GetProperty(nameof(Nullable<bool>.HasValue));
                return Expression.MakeMemberAccess(expr, hasValueProperty);
            }
            else if (expr.Type.IsClass || expr.Type.IsInterface)
            {
                return Expression.Call(null, ReferenceEqualsMethod, expr, Expression.Constant(null));
            }
            else
            {
                return Expression.Constant(false);
            }
        }

        public static Expression ReferenceEquals(Expression x, Expression y)
        {
            return Expression.Call(null, ReferenceEqualsMethod, x, y);
        }

        public static Expression Throw<TException>(Expression<Func<TException>> exceptionExpr) => Expression.Throw(exceptionExpr.Body);

        public static Expression BuildGetterExpression(Expression targetExpr, MemberInfo member)
        {
            if (targetExpr is null)
                throw new ArgumentNullException(nameof(targetExpr));
            if (member is null)
                throw new ArgumentNullException(nameof(member));

            return member.MemberType switch
            {
                MemberTypes.Field when member is FieldInfo field => BuildGetterExpression(targetExpr, field),
                MemberTypes.Property when member is PropertyInfo property => BuildGetterExpression(targetExpr, property),
                _ => throw new InvalidOperationException(),
            };
        }

        public static Expression BuildGetterExpression<TTarget, TMember>(Expression targetExpr, Expression<Func<TTarget, TMember>> getterExpr)
        {
            if (targetExpr is null)
                throw new ArgumentNullException(nameof(targetExpr));
            if (getterExpr is null)
                throw new ArgumentNullException(nameof(getterExpr));

            var member = ReflectionHelpers.From<TTarget>().CaptureFieldOrProperty(getterExpr);

            return BuildGetterExpression(targetExpr, member);
        }

        public static Expression BuildGetterExpression(Expression targetExpr, FieldInfo field)
        {
            if (targetExpr is null)
                throw new ArgumentNullException(nameof(targetExpr));
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (!field.DeclaringType.IsAssignableFrom(targetExpr.Type))
                throw new ArgumentException();

            return Expression.Field(targetExpr, field);
        }

        public static Expression BuildGetterExpression(Expression targetExpr, PropertyInfo property)
        {
            if (targetExpr is null)
                throw new ArgumentNullException(nameof(targetExpr));
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (!property.DeclaringType.IsAssignableFrom(targetExpr.Type))
                throw new ArgumentException();

            if (property.GetMethod is not null)
            {
                return Expression.Call(targetExpr, property.GetMethod);
            }
            else
            {
                var backingFieldName = $"<{property.Name}>k__BackingField";
                var field = targetExpr.Type.GetField(backingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (field is not null)
                {
                    return BuildGetterExpression(targetExpr, field);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static Expression BuildSetterExpression(Expression targetExpr, FieldInfo field, Expression valueExpr)
        {
            // TODO: Make sure the field belongs to TTarget

            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (targetExpr is null)
                throw new ArgumentNullException(nameof(targetExpr));
            if (valueExpr is null)
                throw new ArgumentNullException(nameof(valueExpr));
            if (!field.DeclaringType.IsAssignableFrom(targetExpr.Type))
                throw new ArgumentException();

            var accessExpr = Expression.MakeMemberAccess(targetExpr, field);
            return Expression.Assign(accessExpr, valueExpr);
        }

        public static Expression BuildSetterExpression(Expression targetExpr, PropertyInfo property, Expression valueExpr)
        {
            // TODO: Make sure the field belongs to TTarget

            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (targetExpr is null)
                throw new ArgumentNullException(nameof(targetExpr));
            if (valueExpr is null)
                throw new ArgumentNullException(nameof(valueExpr));
            if (!property.DeclaringType.IsAssignableFrom(targetExpr.Type))
                throw new ArgumentException();

            if (property.SetMethod is not null)
            {
                return Expression.Call(targetExpr, property.SetMethod, valueExpr);
            }
            else
            {
                var backingFieldName = $"<{property.Name}>k__BackingField";
                var field = targetExpr.Type.GetField(backingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (field is not null)
                {
                    return BuildSetterExpression(targetExpr, field, valueExpr);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

}
