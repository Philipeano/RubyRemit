using Microsoft.EntityFrameworkCore;
using RubyRemit.Domain.Interfaces;

namespace RubyRemit.Infrastructure
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly RubyRemitContext _dbContext;
        private readonly DbSet<T> _dbSet;

        protected DbSet<T> DbSet => _dbSet ?? _dbContext.Set<T>();


        public GenericRepository(RubyRemitContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }


        public T Add(T entity)
        {
            DbSet.Add(entity);
            return entity;
        }
    }
}
