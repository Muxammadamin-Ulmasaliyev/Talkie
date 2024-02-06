using Microsoft.EntityFrameworkCore;
using Talkie.Domain;
using Talkie.Domain.Entities;

namespace Talkie.Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _appDbContext;
        public PostRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Post> Create(Post post)
        {
            await _appDbContext.Posts.AddAsync(post);
            await _appDbContext.SaveChangesAsync();
            return post;
        }

        public async Task<bool> Delete(int id)
        {
            var postToDelete =  await _appDbContext.Posts.FindAsync(id);
            if(postToDelete != null)
            {
                _appDbContext.Posts.Remove(postToDelete);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Post> Get(int id)
        {
            return await _appDbContext.Posts.FindAsync(id);
        }


        public async Task<IEnumerable<Post>> GetAll()
        {
            return await _appDbContext.Posts.ToListAsync();
        }

        public async Task<Post> GetAsNoTracking(int id)
        {
            return await _appDbContext.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<string> GetImageUrlById(int id)
        {
            var post = await _appDbContext.Posts.FindAsync(id);
            return post.ImageUrl;
        }

        public async Task<IEnumerable<Post>> GetPostsByUserId(string userId)
        {
            var posts = await _appDbContext.Posts.Where(p => p.AppUserId == userId).ToListAsync();
            return posts;
        }

        

        public async Task<Post> Update(int id, Post post)
        {
            var updatedPost =  _appDbContext.Posts.Attach(post);
            updatedPost.State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();
            return post;
        }
    }
}
