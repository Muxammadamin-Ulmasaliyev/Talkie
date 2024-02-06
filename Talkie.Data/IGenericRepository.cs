namespace Talkie.Data
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Create(T item);
        Task<IEnumerable<T>> GetAll(T item);
        Task<T> Get(int id);
        Task<T> Update(int id, T item);
        Task<bool> Delete(int id);
    }
}
