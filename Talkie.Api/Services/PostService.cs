using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talkie.Api.Infrastructure;
using Talkie.Api.Models;
using Talkie.Api.Profiles;
using Talkie.Data.Repositories;
using Talkie.Domain.Entities;

namespace Talkie.Api.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAzureBlobStorageService _azureBlobStorageService;
        public PostService(IPostRepository postRepository, UserManager<AppUser> userManager, IAzureBlobStorageService azureBlobStorageService)
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _azureBlobStorageService = azureBlobStorageService;
        }

        public async Task<PostModel> Create(UpsertPostModel model)
        {

            model.ImageUrl = (await _azureBlobStorageService.UpsertPostImage(model.AppUserId, model.Image)).Message;
            var post = Mapper.Map(model);
            var createdPost = await _postRepository.Create(post);
            return Mapper.Map(createdPost);

        }

        public async Task<bool> DeletePost(int id)
        {
            return await _postRepository.Delete(id);
        }

        public async Task<bool> DislikePost(int postId)
        {
            var post = await _postRepository.Get(postId);
            if (post != null)
            {
                post.LikeCount--;
                await _postRepository.Update(postId, post);
                return true;
            }
            return false;
        }

        public async Task<PostModel> Get(int id)
        {
            var postFromDb = await _postRepository.Get(id);
            if (postFromDb != null)
            {
                return Mapper.Map(postFromDb);
            }
            return null;
        }

        public async Task<IEnumerable<PostModel>> GetAll()
        {
            var postsFromDb = await _postRepository.GetAll();
            var models = new List<PostModel>();
            foreach (var post in postsFromDb)
            {
                models.Add(Mapper.Map(post));
            }
            return models;
        }

        public async Task<string> GetImageUrlById(int id)
        {
            throw new NotImplementedException();
        }

        // shouldbe improved
        public async Task<IEnumerable<PostModel>> GetPostsByUsername(string username)
        {
            var users = await _userManager.Users.Include(u => u.Posts).ToListAsync();
            var user = users.Find(u => u.UserName == username);
            if (user == null)
            {
                throw new Exception($"User with {username} username not found!");
            }
            var models = new List<PostModel>();
            // var postsFromDb = await _postRepository.GetPostsByUserId(user.Id);
            var postsFromDb = user.Posts;

            foreach (var post in postsFromDb)
            {
                models.Add(Mapper.Map(post));
            }
            return models;
        }

        public async Task<bool> LikePost(int postId)
        {
            var post = await _postRepository.Get(postId);
            if (post != null)
            {
                post.LikeCount++;
                await _postRepository.Update(postId, post);
                return true;
            }
            return false;

        }

        public async Task<PostModel> Update(int id, UpsertPostModel model)
        {
            var postToUpdate = await _postRepository.GetAsNoTracking(id);
            var postModel = Mapper.Map(id, model);
            var updatedPost = await _postRepository.Update(id, postModel);
            return Mapper.Map(updatedPost);
        }
    }
}
