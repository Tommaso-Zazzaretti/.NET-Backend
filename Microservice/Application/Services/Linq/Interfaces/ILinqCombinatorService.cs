using System.Linq.Expressions;

namespace Microservice.Application.Services.Linq.Interfaces
{
    public interface ILinqCombinatorService
    {
        public Expression<Func<T, bool>>? And<T>(Expression<Func<T, bool>>? A, Expression<Func<T, bool>>? B) where T : class;
        public Expression<Func<T, bool>>? Or<T> (Expression<Func<T, bool>>? A, Expression<Func<T, bool>>? B) where T : class;
        public Expression<Func<T, bool>>? Not<T>(Expression<Func<T, bool>>? A) where T : class;
    }
}
