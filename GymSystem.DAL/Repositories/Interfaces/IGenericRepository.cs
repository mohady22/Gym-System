using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity ,new()
    {
        Task<IEnumerable<TEntity>> GetAll(bool IsTracked,CancellationToken ct = default);
        Task<TEntity?> GetById(int id, CancellationToken ct = default);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);
        Task<int> CompleteAsync();
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,bool IsTracked =false,CancellationToken ct=default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,CancellationToken ct=default);
    }
}
