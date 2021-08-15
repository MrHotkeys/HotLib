using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace HotLib.Expressions
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

        /// <summary>
        /// Captures an expression that creates an exception and uses it to create a throw expression.
        /// </summary>
        /// <typeparam name="TException">The type of exception that will be thrown.</typeparam>
        /// <param name="exceptionExpr">The exception creation expression to capture - can be any expression that returns an exception.</param>
        /// <returns>The created throw expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionExpr"/> is null. </exception>
        public static Expression Throw<TException>(Expression<Func<TException>> exceptionExpr) =>
            Throw(exceptionExpr, exceptionExpr?.Type ?? typeof(void));

        /// <summary>
        /// Captures an expression that creates an exception and uses it to create a throw expression.
        /// </summary>
        /// <typeparam name="TException">The type of exception that will be thrown.</typeparam>
        /// <param name="exceptionExpr">The exception creation expression to capture - can be any expression that returns an exception.</param>
        /// <param name="type">The type to set the throw expresion as.</param>
        /// <returns>The created throw expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionExpr"/> or <paramref name="type"/> is null. </exception>
        public static Expression Throw<TException>(Expression<Func<TException>> exceptionExpr, Type type) =>
            exceptionExpr is null ? throw new ArgumentNullException(nameof(exceptionExpr)) :
            type is null ? throw new ArgumentNullException(nameof(type)) :
            Expression.Throw(exceptionExpr.Body, type);

        /// <summary>
        /// Captures an expression that creates an exception and uses it to create a throw expression.
        /// </summary>
        /// <remarks>This overload allows for a parameter to be defined by the given expression - the corresponding
        ///     <see cref="ParameterExpression"/> value of this in the tree will be substituted out for the given expression
        ///     via <see cref="ExpressionParameterSubstitutionVisitor"/>. This allows for some other data from the expression to
        ///     be captured and used when throwing the exception.</remarks>
        /// <typeparam name="T">The type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="TException">The type of exception that will be thrown.</typeparam>
        /// <param name="exceptionExpr">The exception creation expression to capture - can be any expression that returns an exception.</param>
        /// <param name="expr">The expression that will be substituted into the created expression over
        ///     any reference to the parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="type">The type to set the throw expresion as.</param>
        /// <returns>The created throw expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionExpr"/>, <paramref name="expr"/> is null. </exception>
        public static Expression Throw<T, TException>(Expression<Func<T, TException>> exceptionExpr, Expression expr) =>
            Throw(exceptionExpr, expr, exceptionExpr?.Type ?? typeof(void));

        /// <summary>
        /// Captures an expression that creates an exception and uses it to create a throw expression.
        /// </summary>
        /// <remarks>This overload allows for a parameter to be defined by the given expression - the corresponding
        ///     <see cref="ParameterExpression"/> value of this in the tree will be substituted out for the given expression
        ///     via <see cref="ExpressionParameterSubstitutionVisitor"/>. This allows for some other data from the expression to
        ///     be captured and used when throwing the exception.</remarks>
        /// <typeparam name="T">The type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="TException">The type of exception that will be thrown.</typeparam>
        /// <param name="exceptionExpr">The exception creation expression to capture - can be any expression that returns an exception.</param>
        /// <param name="expr">The expression that will be substituted into the created expression over
        ///     any reference to the parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="type">The type to set the throw expresion as.</param>
        /// <returns>The created throw expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionExpr"/>, <paramref name="expr"/>,
        ///     or <paramref name="type"/> is null. </exception>
        public static Expression Throw<T, TException>(Expression<Func<T, TException>> exceptionExpr, Expression expr, Type type) =>
            exceptionExpr is null ? throw new ArgumentNullException(nameof(exceptionExpr)) :
            expr is null ? throw new ArgumentNullException(nameof(expr)) :
            type is null ? throw new ArgumentNullException(nameof(type)) :
            Expression.Throw(ExpressionParameterSubstitutionVisitor.VisitLambda(exceptionExpr, expr), type);

        /// <summary>
        /// Captures an expression that creates an exception and uses it to create a throw expression.
        /// </summary>
        /// <remarks>This overload allows for a number of parameters to be defined by the given expression - the corresponding
        ///     <see cref="ParameterExpression"/> values of these in the tree will be substituted out for the given expressions
        ///     via <see cref="ExpressionParameterSubstitutionVisitor"/>. This allows for some other data from the expression to
        ///     be captured and used when throwing the exception.</remarks>
        /// <typeparam name="T1">The first type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="T2">The second type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="TException">The type of exception that will be thrown.</typeparam>
        /// <param name="exceptionExpr">The exception creation expression to capture - can be any expression that returns an exception.</param>
        /// <param name="expr1">The expression that will be substituted into the created expression over
        ///     any reference to the first parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="expr2">The expression that will be substituted into the created expression over
        ///     any reference to the second parameter of <paramref name="exceptionExpr"/>.</param>
        /// <returns>The created throw expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionExpr"/>, <paramref name="expr1"/>,
        ///     or <paramref name="expr2"/> is null. </exception>
        public static Expression Throw<T1, T2, TException>(Expression<Func<T1, T2, TException>> exceptionExpr,
                                                           Expression expr1, Expression expr2) =>
            Throw(exceptionExpr, expr1, expr2, exceptionExpr?.Type ?? typeof(void));

        /// <summary>
        /// Captures an expression that creates an exception and uses it to create a throw expression.
        /// </summary>
        /// <remarks>This overload allows for a number of parameters to be defined by the given expression - the corresponding
        ///     <see cref="ParameterExpression"/> values of these in the tree will be substituted out for the given expressions
        ///     via <see cref="ExpressionParameterSubstitutionVisitor"/>. This allows for some other data from the expression to
        ///     be captured and used when throwing the exception.</remarks>
        /// <typeparam name="T1">The first type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="T2">The second type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="TException">The type of exception that will be thrown.</typeparam>
        /// <param name="exceptionExpr">The exception creation expression to capture - can be any expression that returns an exception.</param>
        /// <param name="expr1">The expression that will be substituted into the created expression over
        ///     any reference to the first parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="expr2">The expression that will be substituted into the created expression over
        ///     any reference to the second parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="type">The type to set the throw expresion as.</param>
        /// <returns>The created throw expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionExpr"/>, <paramref name="expr1"/>,
        ///     <paramref name="expr2"/>, or <paramref name="type"/> is null. </exception>
        public static Expression Throw<T1, T2, TException>(Expression<Func<T1, T2, TException>> exceptionExpr,
                                                           Expression expr1, Expression expr2, Type type) =>
            exceptionExpr is null ? throw new ArgumentNullException(nameof(exceptionExpr)) :
            expr1 is null ? throw new ArgumentNullException(nameof(expr1)) :
            expr2 is null ? throw new ArgumentNullException(nameof(expr2)) :
            type is null ? throw new ArgumentNullException(nameof(type)) :
            Expression.Throw(ExpressionParameterSubstitutionVisitor.VisitLambda(exceptionExpr, expr1, expr2), type);

        /// <summary>
        /// Captures an expression that creates an exception and uses it to create a throw expression.
        /// </summary>
        /// <remarks>This overload allows for a number of parameters to be defined by the given expression - the corresponding
        ///     <see cref="ParameterExpression"/> values of these in the tree will be substituted out for the given expressions
        ///     via <see cref="ExpressionParameterSubstitutionVisitor"/>. This allows for some other data from the expression to
        ///     be captured and used when throwing the exception.</remarks>
        /// <typeparam name="T1">The first type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="T2">The second type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="T3">The third type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="TException">The type of exception that will be thrown.</typeparam>
        /// <param name="exceptionExpr">The exception creation expression to capture - can be any expression that returns an exception.</param>
        /// <param name="expr1">The expression that will be substituted into the created expression over
        ///     any reference to the first parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="expr2">The expression that will be substituted into the created expression over
        ///     any reference to the second parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="expr3">The expression that will be substituted into the created expression over
        ///     any reference to the third parameter of <paramref name="exceptionExpr"/>.</param>
        /// <returns>The created throw expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionExpr"/>, <paramref name="expr1"/>,
        ///     <paramref name="expr2"/>, or <paramref name="expr3"/> is null. </exception>
        public static Expression Throw<T1, T2, T3, TException>(Expression<Func<T1, T2, T3, TException>> exceptionExpr,
                                                               Expression expr1, Expression expr2, Expression expr3) =>
            Throw(exceptionExpr, expr1, expr2, expr3, exceptionExpr?.Type ?? typeof(void));

        /// <summary>
        /// Captures an expression that creates an exception and uses it to create a throw expression.
        /// </summary>
        /// <remarks>This overload allows for a number of parameters to be defined by the given expression - the corresponding
        ///     <see cref="ParameterExpression"/> values of these in the tree will be substituted out for the given expressions
        ///     via <see cref="ExpressionParameterSubstitutionVisitor"/>. This allows for some other data from the expression to
        ///     be captured and used when throwing the exception.</remarks>
        /// <typeparam name="T1">The first type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="T2">The second type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="T3">The third type of expression that will be substituted into the created expression.</typeparam>
        /// <typeparam name="TException">The type of exception that will be thrown.</typeparam>
        /// <param name="exceptionExpr">The exception creation expression to capture - can be any expression that returns an exception.</param>
        /// <param name="expr1">The expression that will be substituted into the created expression over
        ///     any reference to the first parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="expr2">The expression that will be substituted into the created expression over
        ///     any reference to the second parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="expr3">The expression that will be substituted into the created expression over
        ///     any reference to the third parameter of <paramref name="exceptionExpr"/>.</param>
        /// <param name="type">The type to set the throw expresion as.</param>
        /// <returns>The created throw expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionExpr"/>, <paramref name="expr1"/>,
        ///     <paramref name="expr2"/>, <paramref name="expr3"/>, or <paramref name="type"/> is null. </exception>
        public static Expression Throw<T1, T2, T3, TException>(Expression<Func<T1, T2, T3, TException>> exceptionExpr,
                                                               Expression expr1, Expression expr2, Expression expr3, Type type) =>
            exceptionExpr is null ? throw new ArgumentNullException(nameof(exceptionExpr)) :
            expr1 is null ? throw new ArgumentNullException(nameof(expr1)) :
            expr2 is null ? throw new ArgumentNullException(nameof(expr2)) :
            expr3 is null ? throw new ArgumentNullException(nameof(expr3)) :
            type is null ? throw new ArgumentNullException(nameof(type)) :
            Expression.Throw(ExpressionParameterSubstitutionVisitor.VisitLambda(exceptionExpr, expr1, expr2, expr3), type);

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

        public static Expression GetConditionalNullExpression(Expression checkExpr, Expression ifNonNullExpr, Expression ifNullExpr)
        {
            if (!checkExpr.Type.IsClass && !checkExpr.Type.IsInterface && Nullable.GetUnderlyingType(checkExpr.Type) == null)
                return ifNonNullExpr; // Can't be null, no check needed

            return Expression.Condition(
                test: Expression.Equal(
                    left: checkExpr,
                    right: Expression.Constant(null, checkExpr.Type)),
                ifTrue: ifNullExpr,
                ifFalse: ifNonNullExpr);
        }

        public static Expression GetConditionalNullExpression<TException>(Expression checkExpr, Expression ifNonNullExpr, Expression<Func<TException>> newExceptionExpr)
            where TException : Exception
        {
            return GetConditionalNullExpression(
                checkExpr: checkExpr,
                ifNonNullExpr: ifNonNullExpr,
                ifNullExpr: Throw(newExceptionExpr, ifNonNullExpr.Type));
        }

        public static bool TryMakeConvert(Expression expr, Type type, [NotNullWhen(true)] out UnaryExpression? convertedExpr)
        {
            try
            {
                convertedExpr = Expression.Convert(expr, type);
                return true;
            }
            catch (InvalidOperationException)
            {
                convertedExpr = default;
                return false;
            }
        }

        public static Expression CaptureBody(Expression<Action> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr.Body;
        }

        public static Expression CaptureBody<T>(Expression<Func<T>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr.Body;
        }

        public static Expression<Action> Capture(Expression<Action> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        public static Expression<Action<T>> Capture<T>(Expression<Action<T>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        public static Expression<Action<T1, T2>> Capture<T1, T2>(Expression<Action<T1, T2>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        public static Expression<Action<T1, T2, T3>> Capture<T1, T2, T3>(Expression<Action<T1, T2, T3>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        public static Expression<Func<TOut>> Capture<TOut>(Expression<Func<TOut>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        public static Expression<Func<T, TOut>> Capture<T, TOut>(Expression<Func<T, TOut>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        public static Expression<Func<T1, T2, TOut>> Capture<T1, T2, TOut>(Expression<Func<T1, T2, TOut>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        public static Expression<Func<T1, T2, T3, TOut>> Capture<T1, T2, T3, TOut>(Expression<Func<T1, T2, T3, TOut>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }
    }
}
