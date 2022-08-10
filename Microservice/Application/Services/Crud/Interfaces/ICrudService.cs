using System.Linq.Expressions;

namespace Microservice.Application.Services.Crud.Interfaces
{
    public interface ICrudService<T>
    {
        //CRUD
        public Task<T>  Create(T EntityToAdd);
        public Task<T?> Retrieve(Expression<Func<T,bool>> WherePredicate, params Expression<Func<T, dynamic?>>[] NavigationProperties);
        public Task<T>  Update(T EntityToUpdate);
        public Task<T?> Delete(Expression<Func<T, bool>> WherePredicate);

        //UTILS
        public Task<IEnumerable<T>> RetrieveAll(Expression<Func<T, bool>> WherePredicate, params Expression<Func<T, dynamic?>>[] NavigationProperties);
        public Task<T> UpdatePartially(T EntityToUpdate, Action<T> UpdateAction);
        public Task<T> UpdatePartially(Expression<Func<T, bool>> WherePredicate, Action<T> UpdateAction);
    }
}
