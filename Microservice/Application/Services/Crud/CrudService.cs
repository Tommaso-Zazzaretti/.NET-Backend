using Microservice.Application.Services.Crud.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Microservice.Application.Services.Crud
{
    public class CrudService<T,TDbCtx> : ICrudService<T>   where T : class   where TDbCtx : DbContext
    {
        private readonly TDbCtx _ctx;

        public CrudService(TDbCtx databaseContext) {
            this._ctx = databaseContext;
        }

        public async Task<T> Create(T EntityToAdd) {
            if(EntityToAdd == null) { throw new ArgumentNullException(nameof(EntityToAdd)); }
            EntityEntry<T> AddedEntity = await this._ctx.Set<T>().AddAsync(EntityToAdd);
            int AddedRows = await this._ctx.SaveChangesAsync();
            if (AddedRows == 0) { throw new Exception("Error during create"); }
            return AddedEntity.Entity;
        }

        public async Task<T?> Retrieve(Expression<Func<T, bool>> WherePredicate, params Expression<Func<T, dynamic?>>[] NavigationProperties)
        {
            if (WherePredicate == null) { throw new ArgumentNullException(nameof(WherePredicate)); }
            //Init the query (FROM)
            IQueryable<T> GetEntityByPredicateQuery = this._ctx.Set<T>();
            //Include Navigation Properties (JOIN)
            foreach (var navigationProperty in NavigationProperties) {
                GetEntityByPredicateQuery = GetEntityByPredicateQuery.Include(navigationProperty);
            }
            //Add filter clause (WHERE)
            GetEntityByPredicateQuery = GetEntityByPredicateQuery.Where(WherePredicate);
            //Execute the query
            return await GetEntityByPredicateQuery.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<T> Update(T EntityToUpdate)
        {
            if (EntityToUpdate == null){ throw new ArgumentNullException(nameof(EntityToUpdate)); }
            EntityEntry<T> UpdatedEntity = this._ctx.Set<T>().Update(EntityToUpdate);
            int UpdatedRows = await this._ctx.SaveChangesAsync();
            if (UpdatedRows == 0) { throw new Exception("Error during update"); }
            return UpdatedEntity.Entity;
        }

        public async Task<T?> Delete(Expression<Func<T, bool>> WherePredicate)
        {
            if (WherePredicate == null) { throw new ArgumentNullException(nameof(WherePredicate)); }
            T? EntityToDelete = await this.Retrieve(WherePredicate);
            if(EntityToDelete == null) { return null; }
            EntityEntry<T> DeletedEntity = this._ctx.Set<T>().Remove(EntityToDelete);
            int DeletedRows = await this._ctx.SaveChangesAsync();
            if (DeletedRows == 0) { throw new Exception("Error during delete"); }
            return DeletedEntity.Entity;
        }
    }
}
