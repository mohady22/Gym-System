using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext dbContext;
        private readonly Dictionary<string, object> _repo = [];

        public UnitOfWork(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if (_repo.TryGetValue(typeName, out object oldRepository))
                return (IGenericRepository<TEntity>)oldRepository;

            var NewRepository = new GenericRepository<TEntity>(dbContext);
            _repo[typeName] = NewRepository;
            return NewRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

    }
}
