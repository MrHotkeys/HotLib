using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using HotLib.DotNetExtensions;

namespace HotLib.Equality
{
    /// <summary>
    /// Used to generate <see cref="EqualityFunction{T}"/> delegates based off of given types.
    /// </summary>
    public class EqualityFunctionGenerator : IEqualityFunctionGenerator
    {
        /// <summary>
        /// Instantiates a new <see cref="EqualityFunctionGenerator"/>.
        /// </summary>
        public EqualityFunctionGenerator()
        { }

        /// <summary>
        /// Gets a <see cref="EqualityFunction{T}"/> for comparing objects of the given <see cref="Type"/> for equality.
        /// </summary>
        /// <param name="declaredMembers">An array of all the members declared in the given type
        ///     to include in determining equality.</param>
        /// <param name="inheritedMembers">An array of all the members inherited by the given type
        ///     from base types to include in determining equality.</param>
        /// <returns>The created equality function.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaredMembers"/>
        ///     or <paramref name="inheritedMembers"/> is null.</exception>
        public virtual EqualityFunction<T> BuildEqualityFunction<T>(MemberInfo[] declaredMembers, MemberInfo[] inheritedMembers)
        {
            if (declaredMembers == null)
                throw new ArgumentNullException(nameof(declaredMembers));
            if (inheritedMembers == null)
                throw new ArgumentNullException(nameof(inheritedMembers));

            // Represents the parameters to the hash code function - the two target objects, whether or not
            // to include members from base types, and the equality comparer for comparing member values
            var x_targetParameterExpression = Expression.Parameter(typeof(T));
            var y_targetParameterExpression = Expression.Parameter(typeof(T));
            var includeInheritedParameterExpression = Expression.Parameter(typeof(bool));
            var equalityComparerParameterExpression = Expression.Parameter(typeof(IUniversalEqualityComparer));

            // Get the expression for the comparison between just the declared members and create a local variable to store it in
            var declaredComparisonExpression = AppendMemberComparisonExpression<T>(Expression.Constant(true, typeof(bool)),
                                                                                   declaredMembers,
                                                                                   x_targetParameterExpression,
                                                                                   y_targetParameterExpression,
                                                                                   equalityComparerParameterExpression);
            var declaredComparisonVariableExpression = Expression.Variable(typeof(bool), "declaredEquals");
            var declaredComparisonVariableAssignExpression = Expression.Assign(declaredComparisonVariableExpression,
                                                                               declaredComparisonExpression);

            // Get the expression for the comparison with declared and inherited values
            var allComparisonExpression = AppendMemberComparisonExpression<T>(declaredComparisonExpression,
                                                                              inheritedMembers,
                                                                              x_targetParameterExpression,
                                                                              y_targetParameterExpression,
                                                                              equalityComparerParameterExpression);

            // The label used to return out of the block expression that represents the body of the method
            var returnLabel = Expression.Label(typeof(bool));
            var returnExpression = Expression.Label(returnLabel,
                                                    Expression.Constant(false, typeof(bool)));

            // Branch depending on whether we are including inherited members
            var ifIncludeInheritedExpression = Expression.IfThenElse(includeInheritedParameterExpression,
                                                                     Expression.Return(returnLabel, allComparisonExpression),
                                                                     Expression.Return(returnLabel, declaredComparisonExpression));

            // Holds the group of expressions as we're working with it and conditionally adding more
            var expressions = new List<Expression>()
            {
                declaredComparisonVariableAssignExpression,
                ifIncludeInheritedExpression,
                returnExpression,
            };

            // If there are no declared members, add a guard clause to the beginning of the hash code function so that if false
            // is passed for including inherited members, an exception would be thrown (otherwise there'd be nothing to hash)
            if (declaredMembers.Length == 0)
            {
                var typeExpression = Expression.Constant(typeof(T), typeof(Type));
                var exceptionExpression = Expression.New(OnlyInheritedMembersException.ConstructorInfo, typeExpression);
                var throwExpression = Expression.Throw(exceptionExpression);
                var guardClauseExpression = Expression.IfThen(Expression.Not(includeInheritedParameterExpression),
                                                              throwExpression);
                expressions.Insert(0, guardClauseExpression);
            }

            // The block expression that represents the body of the generated method
            var variables = Enumerable.Repeat(declaredComparisonVariableExpression, 1);
            var blockExpression = Expression.Block(variables, expressions);

            return Expression.Lambda<EqualityFunction<T>>(blockExpression,
                                                          x_targetParameterExpression,
                                                          y_targetParameterExpression,
                                                          includeInheritedParameterExpression,
                                                          equalityComparerParameterExpression)
                             .Compile();
        }

        /// <summary>
        /// Creates and returns an expression for appending the given expression for equality
        /// comparison with more member value comparisons.
        /// </summary>
        /// <typeparam name="T">The type of target object that the hash code is being created for.</typeparam>
        /// <param name="comparisonExpression">The equality comparison expression to append.</param>
        /// <param name="members">The members to append the hash code with.</param>
        /// <param name="x_targetParameterExpression">The expression for the first target object as a parameter.</param>
        /// <param name="y_targetParameterExpression">The expression for the second target object as a parameter.</param>
        /// <param name="equalityComparerExpression">The expression for the equality comparer as a parameter.</param>
        /// <returns>The appended equality comparison expression.</returns>
        protected virtual Expression AppendMemberComparisonExpression<T>(Expression comparisonExpression,
                                                                         IEnumerable<MemberInfo> members,
                                                                         ParameterExpression x_targetParameterExpression,
                                                                         ParameterExpression y_targetParameterExpression,
                                                                         ParameterExpression equalityComparerExpression)
        {
            foreach (var member in members)
            {
                // Represents the target objects, as unboxed from the parameter to their actual type
                // We unbox here since we can unbox to the member's declaring type to be able to find hidden members
                var x_targetExpression = member.DeclaringType != typeof(T) ?
                                         Expression.Convert(x_targetParameterExpression, member.DeclaringType) :
                                         x_targetParameterExpression as Expression;
                var y_targetExpression = member.DeclaringType != typeof(T) ?
                                         Expression.Convert(y_targetParameterExpression, member.DeclaringType) :
                                         y_targetParameterExpression as Expression;

                // Get the values of the members
                var x_memberAccessExpression = Expression.PropertyOrField(x_targetExpression, member.Name);
                var y_memberAccessExpression = Expression.PropertyOrField(y_targetExpression, member.Name);

                // MethodInfo for IUniversalEqualityComparer.Equals<T>(T x, T y)
                var memberValueType = member.GetFieldOrPropertyType();
                var equalsMethodTypeArgs = new[] { memberValueType };
                var equalsMethod = typeof(IUniversalEqualityComparer).GetMethod(nameof(IUniversalEqualityComparer.Equals))
                                                                     .MakeGenericMethod(equalsMethodTypeArgs);

                // Get the hash for the current member from the default comparer
                var memberEqualsExpression = Expression.Call(equalityComparerExpression,
                                                             equalsMethod,
                                                             x_memberAccessExpression,
                                                             y_memberAccessExpression);

                // Append the equality comparison expression
                if (comparisonExpression == null)
                    comparisonExpression = memberEqualsExpression;
                else
                    comparisonExpression = Expression.AndAlso(comparisonExpression, memberEqualsExpression);
            }

            return comparisonExpression;
        }
    }
}
