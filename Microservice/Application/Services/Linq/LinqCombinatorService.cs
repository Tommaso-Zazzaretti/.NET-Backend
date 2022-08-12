using Microservice.Application.Services.Linq.Interfaces;
using System.Linq.Expressions;

namespace Microservice.Application.Services.Linq
{
    public class LinqCombinatorService : ILinqCombinatorService
    {
        public Expression<Func<T, bool>>? And<T>(Expression<Func<T, bool>>? A, Expression<Func<T, bool>>? B) where T : class
        {
            if (A == null && B == null) { return null; }
            if (A != null && B == null) { return A; }
            if (A == null && B != null) { return B; }
            //Declare a binder object to exchange the B ParameterExpression with the A ParameterExpression
            var Binder = new Binder(B!.Parameters.Single(), A!.Parameters.Single());
            var ABody = A!.Body;
            var BBody = Binder.Visit(B!.Body);
            var AndExpression = Expression.AndAlso(ABody, BBody);
            return Expression.Lambda<Func<T, bool>>(AndExpression, A!.Parameters.Single());
        }

        public Expression<Func<T, bool>>? Or<T>(Expression<Func<T, bool>>? A, Expression<Func<T, bool>>? B) where T : class
        {
            if (A == null && B == null) { return null; }
            if (A != null && B == null) { return A; }
            if (A == null && B != null) { return B; }
            //Declare a binder object to exchange the B ParameterExpression with the A ParameterExpression
            var Binder = new Binder(B!.Parameters.Single(), A!.Parameters.Single());
            var ABody = A!.Body;
            var BBody = Binder.Visit(B!.Body);
            var OrExpression = Expression.OrElse(ABody, BBody);
            return Expression.Lambda<Func<T, bool>>(OrExpression, A!.Parameters.Single());
        }
        public Expression<Func<T, bool>>? Not<T>(Expression<Func<T, bool>>? A) where T : class
        {
            if (A == null) { return null; }
            var NotExpression = Expression.Not(A.Body);
            return Expression.Lambda<Func<T,bool>>(NotExpression, A.Parameters.Single());
        }
    }

    public class Binder : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public Binder(ParameterExpression Old, ParameterExpression New) {
            this._oldParameter = Old; this._newParameter = New;
        }

        //Expression Tree Visit
        protected override Expression VisitParameter(ParameterExpression treeNode) {
            return (treeNode == this._oldParameter) ? this._newParameter : base.VisitParameter(treeNode);
        }
    }
}
