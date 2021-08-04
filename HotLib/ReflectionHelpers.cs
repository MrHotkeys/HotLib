using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HotLib
{
    public static class ReflectionHelpers
    {
        public sealed class ArgMock
        {
            private ArgMock()
            { }

            public T Of<T>() => throw new InvalidOperationException();
        }

        public static Type GetFieldOrPropertyType(this MemberInfo member)
        {
            if (member is null)
                throw new ArgumentNullException(nameof(member));

            return member.MemberType switch
            {
                MemberTypes.Field => (member as FieldInfo)!.FieldType,
                MemberTypes.Property => (member as PropertyInfo)!.PropertyType,
                _ => throw new InvalidOperationException(),
            };
        }

        public static Action<TTarget, TField> BuildSetter<TTarget, TField>(FieldInfo field)
        {
            // TODO: Make sure the field belongs to TTarget

            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (field.FieldType != typeof(TField))
                throw new InvalidOperationException();

            var targetExpr = Expression.Parameter(typeof(TTarget));
            var valueExpr = Expression.Parameter(typeof(TField));

            var setterExpr = ExpressionHelpers.BuildSetterExpression(targetExpr, field, valueExpr);

            return Expression.Lambda<Action<TTarget, TField>>(setterExpr, targetExpr, valueExpr)
                             .Compile();
        }

        public static Action<TTarget, TProperty> BuildSetter<TTarget, TProperty>(PropertyInfo property)
        {
            // TODO: Make sure the property belongs to TTarget

            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (property.PropertyType != typeof(TProperty))
                throw new InvalidOperationException();

            var targetExpr = Expression.Parameter(typeof(TTarget));
            var valueExpr = Expression.Parameter(typeof(TProperty));

            var setterExpr = ExpressionHelpers.BuildSetterExpression(targetExpr, property, valueExpr);

            return Expression.Lambda<Action<TTarget, TProperty>>(setterExpr, targetExpr, valueExpr)
                             .Compile();
        }

        public static Func<TTarget, TField> BuildGetter<TTarget, TField>(FieldInfo field)
        {
            // TODO: Make sure the field belongs to TTarget

            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (field.FieldType != typeof(TField))
                throw new InvalidOperationException();

            var targetExpr = Expression.Parameter(typeof(TTarget));

            var getterExpr = ExpressionHelpers.BuildGetterExpression(targetExpr, field);

            return Expression.Lambda<Func<TTarget, TField>>(getterExpr, targetExpr)
                             .Compile();
        }

        public static Func<TTarget, TProperty> BuildGetter<TTarget, TProperty>(PropertyInfo property)
        {
            // TODO: Make sure the property belongs to TTarget

            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (property.PropertyType != typeof(TProperty))
                throw new InvalidOperationException();

            var targetExpr = Expression.Parameter(typeof(TTarget));

            var getterExpr = ExpressionHelpers.BuildGetterExpression(targetExpr, property);

            return Expression.Lambda<Func<TTarget, TProperty>>(getterExpr, targetExpr)
                             .Compile();
        }

        public static MethodInfo CaptureMethod(Action action) =>
            action.Method ?? throw new ArgumentNullException(nameof(action));
        public static MethodInfo CaptureMethod<T>(Action<T> action) =>
            action.Method ?? throw new ArgumentNullException(nameof(action));
        public static MethodInfo CaptureMethod<T1, T2>(Action<T1, T2> action) =>
            action.Method ?? throw new ArgumentNullException(nameof(action));
        public static MethodInfo CaptureMethod<T1, T2, T3>(Action<T1, T2, T3> action) =>
            action.Method ?? throw new ArgumentNullException(nameof(action));
        public static MethodInfo CaptureMethod<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action) =>
            action.Method ?? throw new ArgumentNullException(nameof(action));
        public static MethodInfo CaptureMethod<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action) =>
            action.Method ?? throw new ArgumentNullException(nameof(action));

        public static MethodInfo CaptureMethod<TResult>(Func<TResult> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod(Func<object> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T, TResult>(Func<T, TResult> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T>(Func<T, object> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T1, T2, TResult>(Func<T1, T2, TResult> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T1, T2>(Func<T1, T2, object> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T1, T2, T3>(Func<T1, T2, T3, object> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        public static MethodInfo CaptureMethod<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object> func) =>
            func.Method ?? throw new ArgumentNullException(nameof(func));

        //public static MemberInfo CaptureMember<TMember>(Expression<Func<TMember>> getterExpr)
        //{
        //    if (getterExpr is null)
        //        throw new ArgumentNullException(nameof(getterExpr));

        //    switch (getterExpr.Body.NodeType)
        //    {
        //        case ExpressionType.MemberAccess when getterExpr.Body is MemberExpression memberExpr:
        //            return memberExpr.Member;
        //        case ExpressionType.Call when getterExpr.Body is MethodCallExpression callExpr:
        //            {

        //            }
        //    }
        //}

        private static MemberInfo CaptureFieldOrProperty<TMember>(LambdaExpression getterExpr) =>
            getterExpr is null ?
                throw new ArgumentNullException(nameof(getterExpr)) :
            getterExpr.Body is not MemberExpression memberExpr ?
                throw new ArgumentException("Must be a property or field access!", nameof(getterExpr)) :
            memberExpr.Member;

        public static MemberInfo CaptureFieldOrProperty<TMember>(Expression<Func<TMember>> getterExpr) =>
            CaptureFieldOrProperty<TMember>(getterExpr as LambdaExpression);

        private static FieldInfo CaptureField<TField>(LambdaExpression getterExpr) =>
            CaptureFieldOrProperty<TField>(getterExpr) is not FieldInfo field ?
                throw new ArgumentException("Must be a field access!", nameof(getterExpr)) :
            field;

        public static FieldInfo CaptureField<TField>(Expression<Func<TField>> getterExpr) =>
            CaptureField<TField>(getterExpr as LambdaExpression);

        private static PropertyInfo CaptureProperty<TProperty>(LambdaExpression getterExpr) =>
            CaptureFieldOrProperty<TProperty>(getterExpr) is not PropertyInfo property ?
                throw new ArgumentException("Must be a property access!", nameof(getterExpr)) :
            property;

        public static PropertyInfo CaptureProperty<TProperty>(Expression<Func<TProperty>> getterExpr) =>
            CaptureProperty<TProperty>(getterExpr as LambdaExpression);

        public static ConstructorInfo CaptureConstructor<TReturn>(Expression<Func<TReturn>> expr) =>
            expr is null ?
                throw new ArgumentNullException(nameof(expr)) :
            expr.Body is not NewExpression newExpr ?
                throw new ArgumentException("Must be a new expression!", nameof(expr)) :
            newExpr.Constructor;

        public static ConstructorInfo CaptureConstructor<TReturn>(Expression<Func<ArgMock, TReturn>> expr) =>
            expr is null ? throw new ArgumentNullException(nameof(expr)) :
            expr.Body is not NewExpression newExpr ? throw new ArgumentException() :
            newExpr.Constructor;

        public static MemberCaptureContext<T> From<T>() => MemberCaptureContext<T>.Instance;

        public class MemberCaptureContext<TFrom>
        {
            public static MemberCaptureContext<TFrom> Instance { get; } = new MemberCaptureContext<TFrom>();

            protected MemberCaptureContext()
            { }

            public MemberInfo CaptureFieldOrProperty<TMember>(Expression<Func<TFrom, TMember>> getterExpr) =>
                ReflectionHelpers.CaptureFieldOrProperty<TMember>(getterExpr);

            public FieldInfo CaptureField<TField>(Expression<Func<TFrom, TField>> getterExpr) =>
                ReflectionHelpers.CaptureField<TField>(getterExpr);

            public PropertyInfo CaptureProperty<TProperty>(Expression<Func<TFrom, TProperty>> getterExpr) =>
                ReflectionHelpers.CaptureProperty<TProperty>(getterExpr);

            /// <summary>
            /// Given a <see cref="LambdaExpression"/> whose body returns a method as a delegate (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// extracts and returns the <see cref="MethodInfo"/> from that delegate creation.
            /// </summary>
            /// <remarks>We need to use expression trees in <see cref="MemberCaptureContext{TFrom}"/> instead of working 
            ///     with the given Action/Func getters directly since we won't have an instance to work with.</remarks>
            /// <param name="expr">The expression to extract from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            /// <exception cref="ArgumentException"><paramref name="expr"/> does not follow the above format.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="expr"/> is null.</exception>
            private MethodInfo CaptureMethod(LambdaExpression expr)
            {
                if (expr is null)
                    throw new ArgumentNullException(nameof(expr));

                // The expression will not just be a method call, it will be a delegate creation based off that method,
                // so we need to extract the call to create the delegate first and then extract the method from that
                if (expr.Body is not UnaryExpression convertExpr || convertExpr.NodeType != ExpressionType.Convert)
                    throw new ArgumentException();
                if (convertExpr.Operand is not MethodCallExpression callExpr || callExpr.Method.Name != nameof(Delegate.CreateDelegate))
                    throw new ArgumentException();
                if (callExpr.Object is not ConstantExpression methodExpr)
                    throw new ArgumentException();

                return (MethodInfo)methodExpr.Value;
            }

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod(Expression<Func<TFrom, Action>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T>(Expression<Func<TFrom, Action<T>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2>(Expression<Func<TFrom, Action<T1, T2>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3>(Expression<Func<TFrom, Action<T1, T2, T3>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3, T4>(Expression<Func<TFrom, Action<T1, T2, T3, T4>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3, T4, T5>(Expression<Func<TFrom, Action<T1, T2, T3, T4, T5>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<TResult>(Expression<Func<TFrom, Func<TResult>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod(Expression<Func<TFrom, Func<object>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T, TResult>(Expression<Func<TFrom, Func<T, TResult>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T>(Expression<Func<TFrom, Func<T, object>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, TResult>(Expression<Func<TFrom, Func<T1, T2, TResult>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2>(Expression<Func<TFrom, Func<T1, T2, object>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3, TResult>(Expression<Func<TFrom, Func<T1, T2, T3, TResult>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3>(Expression<Func<TFrom, Func<T1, T2, T3, object>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3, T4, TResult>(Expression<Func<TFrom, Func<T1, T2, T3, T4, TResult>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3, T4>(Expression<Func<TFrom, Func<T1, T2, T3, T4, object>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3, T4, T5, TResult>(Expression<Func<TFrom, Func<T1, T2, T3, T4, T5, TResult>>> expr) =>
                CaptureMethod(expr as LambdaExpression);

            /// <summary>
            /// Given an expression which returns a reference to a method (e.g. <c>x => x.ToString</c>, <b>not a method call</b>),
            /// returns the method's corresponding <see cref="MethodInfo"/>.
            /// </summary>
            /// <param name="expr">The expression to extract the <see cref="MethodInfo"/> from.</param>
            /// <returns>The extracted <see cref="MethodInfo"/>.</returns>
            public MethodInfo CaptureMethod<T1, T2, T3, T4, T5>(Expression<Func<TFrom, Func<T1, T2, T3, T4, T5, object>>> expr) =>
                CaptureMethod(expr as LambdaExpression);
        }
    }

}
