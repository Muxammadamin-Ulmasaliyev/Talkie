using Talkie.Domain.Entities;

namespace Talkie.Data.Repositories
{
    public interface IPostRepository
    {
        Task<Post> Create(Post post);
        Task<Post> Update(int id, Post post);   
        Task<IEnumerable<Post>> GetAll();
        Task<Post> Get(int id);
        Task<Post> GetAsNoTracking(int id);

        Task<IEnumerable<Post>> GetPostsByUserId(string userId);
        Task<string> GetImageUrlById(int id);
        Task<bool> Delete(int id);


    }
}
