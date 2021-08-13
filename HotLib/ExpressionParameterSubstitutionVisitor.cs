using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using HotLib.DotNetExtensions;

namespace HotLib
{
    /// <summary>
    /// An <see cref="ExpressionVisitor"/> implementation which invokes a given callback function
    /// for each <see cref="ParameterExpression"/> used in a given <see cref="Expression"/>.
    /// </summary>
    public class ExpressionParameterSubstitutionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Gets the callback function used to make replacements.
        /// </summary>
        public Func<ParameterExpression, Expression> VisitParameterCallback { get; }

        /// <summary>
        /// Instantiates a new <see cref="ExpressionVisitor"/>.
        /// </summary>
        /// <param name="visitParameterCallback">The callback function used to make replacements.</param>
        public ExpressionParameterSubstitutionVisitor(Func<ParameterExpression, Expression> visitParameterCallback) =>
            VisitParameterCallback = visitParameterCallback ?? throw new ArgumentNullException(nameof(visitParameterCallback));

        /// <summary>
        /// Invokes the callback on the given <see cref="ParameterExpression"/>.
        /// </summary>
        /// <param name="node">The expression being visited.</param>
        /// <returns>The <see cref="Expression"/> produced by the callback.</returns>
        protected override Expression VisitParameter(ParameterExpression node) =>
            VisitParameterCallback(node);

        /// <summary>
        /// Visits an <see cref="Expression"/>, replacing all instances of a given <see cref="ParameterExpression"/>
        /// with another <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <param name="old">The parameter expression being replaced.</param>
        /// <param name="replacement">The expression replacing the parameter expression.</param>
        /// <returns>A new <see cref="Expression"/> matching the given expression but with all
        ///     instances of the parameter expression replaced.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> or <paramref name="old"/>
        ///     or <paramref name="replacement"/> is null.</exception>
        public static Expression Visit(Expression expression, ParameterExpression old, Expression replacement)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));
            if (old is null)
                throw new ArgumentNullException(nameof(old));
            if (replacement is null)
                throw new ArgumentNullException(nameof(replacement));

            var visitor = new ExpressionParameterSubstitutionVisitor(expr => expr == old ? replacement : expr);
            return visitor.Visit(expression);
        }

        /// <summary>
        /// Visits the body of a <see cref="LambdaExpression"/> and replaces every <see cref="ParameterExpression"/> used in its body with
        /// its corresponding replacement <see cref="Expression"/> (mapped by parameter position and array index) in the given array.
        /// </summary>
        /// <remarks>Exactly as many replacements must be given as parameters on the lambda.</remarks>
        /// <param name="lambdaExpression">The expression to visit the body of.</param>
        /// <param name="replacements">An array of replacements for each parameter to the lambda.</param>
        /// <returns>A new <see cref="Expression"/> matching the body of the given lambda expression but with all
        ///     instances of the parameter expression replaced.</returns>
        public static Expression VisitLambda(LambdaExpression lambdaExpression, params Expression[] replacements)
        {
            if (lambdaExpression is null)
                throw new ArgumentNullException(nameof(lambdaExpression));
            if (lambdaExpression.Parameters.Count != replacements.Length)
            {
                throw new ArgumentException($"The same number of replacement expressions must be given as parameters " +
                    $"to the lambda (got {replacements.Length}, expected {lambdaExpression.Parameters.Count})!", nameof(replacements));
            }

            var replacementsDictionary = lambdaExpression
                .Parameters
                .SelectWithIndex(p => p)
                .ToDictionary(p => p.Item, p => replacements[p.Index]);

            return Visit(lambdaExpression.Body, replacementsDictionary);
        }

        /// <summary>
        /// Visits an <see cref="Expression"/>, replacing every <see cref="ParameterExpression"/> used with its corresponding
        /// replacement <see cref="Expression"/> in the given dictionary. If the parameter expression doesn't exist as a key
        /// in the dictionary, it is left as-is.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <param name="replacements">A dictionary of replacements, with the original parameter expression as the key.
        ///     If a parameter should be left as-is, it can be omitted from the dictionary.</param>
        /// <returns>A new <see cref="Expression"/> matching the given expression but with all
        ///     instances of the parameter expression replaced.</returns>
        /// <exception cref="ArgumentException"><paramref name="replacements"/> contains null as the replacement for a parameter.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> or <paramref name="replacements"/> is null.</exception>
        public static Expression Visit(Expression expression, Dictionary<ParameterExpression, Expression> replacements)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));
            if (replacements is null)
                throw new ArgumentNullException(nameof(replacements));

            var visitor = new ExpressionParameterSubstitutionVisitor(paramExpr =>
            {
                if (replacements.TryGetValue(paramExpr, out var expr))
                {
                    if (expr is null)
                        throw new ArgumentException($"Replacements dictionary contains null as the replacement for parameter expression {paramExpr}!");

                    return expr;
                }
                else
                {
                    return paramExpr;
                }
            });

            return visitor.Visit(expression);
        }
    }
}
