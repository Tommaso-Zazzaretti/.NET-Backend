using System.Linq.Expressions;

namespace Microservice.Application.Services.Crud.Interfaces
{
    public interface ICrudService<T>
    {
        public Task<T>  Create(T EntityToAdd);
        public Task<T?> Retrieve(Expression<Func<T,bool>> WherePredicate, params Expression<Func<T, dynamic?>>[] NavigationProperties);
        public Task<T>  Update(T EntityToUpdate);
        public Task<T?> Delete(Expression<Func<T, bool>> WherePredicate);
    }
}
