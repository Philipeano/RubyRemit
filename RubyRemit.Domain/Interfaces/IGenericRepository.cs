using System;
using System.Linq;
using System.Linq.Expressions;

namespace RubyRemit.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        T Add(T entity);

        T GetById(long id);

        IQueryable<T> GetAll(Expression<Func<T, bool>> expression);
    }
}