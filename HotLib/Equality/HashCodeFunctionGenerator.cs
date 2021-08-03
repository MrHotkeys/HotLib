using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using HotLib.DotNetExtensions;

namespace HotLib.Equality
{
    /// <summary>
    /// Used to generate <see cref="HashCodeFunction{T}"/> delegates based off of given types.
    /// </summary>
    public class HashCodeFunctionGenerator : IHashCodeFunctionGenerator
    {
        /// <summary>
        /// Instantiates a new <see cref="HashCodeFunctionGenerator"/>.
        /// </summary>
        public HashCodeFunctionGenerator()
        { }

        /// <summary>
        /// Gets a <see cref="HashCodeFunction{T}"/> for hashing objects of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="declaredMembers">An array of all the members declared in the given type
        ///     to include in calculating the hash code.</param>
        /// <param name="inheritedMembers">An array of all the members inherited by the given type
        ///     from base types to include in calculating the hash code.</param>
        /// <returns>The created hash code function.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaredMembers"/>
        ///     or <paramref name="inheritedMembers"/> is null.</exception>
        public virtual HashCodeFunction<T> BuildHashCodeFunction<T>(MemberInfo[] declaredMembers, MemberInfo[] inheritedMembers)
        {
            if (declaredMembers == null)
                throw new ArgumentNullException(nameof(declaredMembers));
            if (inheritedMembers == null)
                throw new ArgumentNullException(nameof(inheritedMembers));

            // Represents the parameters to the hash code function - the target object, whether or not
            // to include members from base types, and the equality comparer for hashing member values
            var targetParameterExpression = Expression.Parameter(typeof(T));
            var includeInheritedParameterExpression = Expression.Parameter(typeof(bool));
            var equalityComparerParameterExpression = Expression.Parameter(typeof(IUniversalEqualityComparer));

            // This is the expression that will give us the hash code
            // We start with a large prime and then mutate it with each member value
            var startHashCode = Expression.Constant(unchecked((int)2166136261), typeof(int));

            // Get the expression for the hash code with just declared values and create a local variable to store it in
            var declaredHashCodeExpression = AppendHashCodeExpression<T>(startHashCode, declaredMembers,
                                                                         targetParameterExpression,
                                                                         equalityComparerParameterExpression);
            var declaredHashCodeVariableExpression = Expression.Variable(typeof(int), "declaredHashCode");
            var declaredHashCodeVariableAssignExpression = Expression.Assign(declaredHashCodeVariableExpression,
                                                                             declaredHashCodeExpression);

            // Get the expression for the hash code with declared and inherited values
            var allHashCodeExpression = AppendHashCodeExpression<T>(declaredHashCodeVariableExpression, inheritedMembers,
                                                                    targetParameterExpression,
                                                                    equalityComparerParameterExpression);

            // The label used to return out of the block expression that represents the body of the method
            var returnLabel = Expression.Label(typeof(int));
            var returnExpression = Expression.Label(returnLabel,
                                                    Expression.Constant(-1, typeof(int)));

            // Branch depending on whether we are including inherited members
            var ifIncludeInheritedExpression = Expression.IfThenElse(includeInheritedParameterExpression,
                                                                     Expression.Return(returnLabel, allHashCodeExpression),
                                                                     Expression.Return(returnLabel, declaredHashCodeVariableExpression));

            // Holds the group of expressions as we're working with it and conditionally adding more
            var expressions = new List<Expression>()
            {
                declaredHashCodeVariableAssignExpression,
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
            var variables = Enumerable.Repeat(declaredHashCodeVariableExpression, 1);
            var blockExpression = Expression.Block(variables, expressions);

            return Expression.Lambda<HashCodeFunction<T>>(blockExpression,
                                                          targetParameterExpression,
                                                          includeInheritedParameterExpression,
                                                          equalityComparerParameterExpression)
                             .Compile();
        }

        /// <summary>
        /// Creates and returns an expression for appending the given
        /// expression for a hash code with more member value mutations.
        /// </summary>
        /// <typeparam name="T">The type of target object that the hash code is being created for.</typeparam>
        /// <param name="hashCodeExpression">The hash code expression to append.</param>
        /// <param name="members">The members to append the hash code with.</param>
        /// <param name="targetParameterExpression">The expression for the target object as a parameter.</param>
        /// <param name="equalityComparerExpression">The expression for the equality comparer as a parameter.</param>
        /// <returns>The appended hash code expression.</returns>
        protected virtual Expression AppendHashCodeExpression<T>(Expression hashCodeExpression, IEnumerable<MemberInfo> members,
                                                                 ParameterExpression targetParameterExpression,
                                                                 ParameterExpression equalityComparerExpression)
        {
            // A constant expression for the prime that we multiply with for each member value
            var hashMultiplyFactorExpression = Expression.Constant(16777619);

            foreach (var member in members)
            {
                if (member.DeclaringType is null)
                    throw new InvalidOperationException();

                // Represents the target object, as unboxed from the parameter to its actual type
                // We unbox here since we can unbox to the member's declaring type to be able to find hidden members
                var targetExpression = member.DeclaringType != typeof(T) ?
                                       Expression.Convert(targetParameterExpression, member.DeclaringType) :
                                       targetParameterExpression as Expression;

                // Get the value of the member
                var memberAccessExpression = Expression.PropertyOrField(targetExpression, member.Name);

                // MethodInfo for IUniversalEqualityComparer.GetHashCode<T>(T obj)
                var getHashCodeTypeParameters = new[] { member.GetFieldOrPropertyType() };
                var getHashCode = typeof(IUniversalEqualityComparer)
                    .GetMethod(nameof(IUniversalEqualityComparer.GetHashCode))
                    ?.MakeGenericMethod(getHashCodeTypeParameters)
                    ?? throw new InvalidOperationException();

                // Get the hash for the current member from the default comparer
                var memberHashExpression = Expression.Call(equalityComparerExpression,
                                                           getHashCode,
                                                           memberAccessExpression);

                // Mutate the hash code based off the hash code for the current member
                // (currentHash * multiplyFactor) ^ memberHash
                hashCodeExpression = Expression.Multiply(hashCodeExpression, hashMultiplyFactorExpression);
                hashCodeExpression = Expression.ExclusiveOr(hashCodeExpression, memberHashExpression);
            }

            return hashCodeExpression;
        }
    }
}
