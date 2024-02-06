using Microsoft.EntityFrameworkCore;
using Talkie.Domain;

namespace Talkie.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal readonly AppDbContext _appDbContext;
        public GenericRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

        public async Task<T> Create(T item)
        {
            await _appDbContext.Set<T>().AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            return item;
        }

        public async Task<bool> Delete(int id)
        {
            var itemToDelete = await _appDbContext.Set<T>().FindAsync(id);
            if (itemToDelete != null)
            {
                _appDbContext.Set<T>().Remove(itemToDelete);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<T> Get(int id)
        {
            return await _appDbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll(T item)
        {
            return await _appDbContext.Set<T>().ToListAsync();
        }

        public async Task<T> Update(int id, T item)
        {
            var updatedItem = _appDbContext.Set<T>().Attach(item);
            updatedItem.State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();
            return item;
        }
    }
}
