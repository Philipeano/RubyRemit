namespace RubyRemit.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        T Add(T entity);
    }
}