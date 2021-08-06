using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HotLib
{
    public class ExpressionParameterSubstitutionVisitor : ExpressionVisitor
    {
        public Func<Expression, Expression> VisitParameterCallback { get; }

        public ExpressionParameterSubstitutionVisitor(Func<Expression, Expression> visitParameterCallback)
        {
            VisitParameterCallback = visitParameterCallback ?? throw new ArgumentNullException(nameof(visitParameterCallback));
        }

        protected override Expression VisitParameter(ParameterExpression node) =>
            VisitParameterCallback(node);

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

        public static Expression VisitLambda(LambdaExpression lambdaExpression, params Expression[] replacements) =>
            lambdaExpression is null ? throw new ArgumentNullException(nameof(lambdaExpression)) :
            Visit(lambdaExpression.Body, lambdaExpression.Parameters, replacements);

        public static Expression Visit(Expression expression, IList<ParameterExpression> old, Expression[] replacements)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));
            if (old is null)
                throw new ArgumentNullException(nameof(old));
            if (replacements is null)
                throw new ArgumentNullException(nameof(replacements));
            if (old.Count != replacements.Length)
                throw new ArgumentException("Must give equal number of replacement expressions!");

            var visitor = new ExpressionParameterSubstitutionVisitor(expr =>
            {
                for (var i = 0; i < old.Count; i++)
                {
                    if (expr == old[i])
                        return replacements[i];
                }

                return expr;
            });

            return visitor.Visit(expression);
        }
    }
}
