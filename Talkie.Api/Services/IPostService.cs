using Talkie.Api.Models;
using Talkie.Domain.Entities;

namespace Talkie.Api.Services
{
    public interface IPostService
    {
        Task<PostModel> Create(UpsertPostModel model);
        Task<PostModel> Update(int id, UpsertPostModel model);
        Task<IEnumerable<PostModel>> GetAll();
        Task<PostModel> Get(int id);
        Task<IEnumerable<PostModel>> GetPostsByUsername(string username);
        Task<string> GetImageUrlById(int id);

        Task<bool> DeletePost(int id);

        Task<bool> LikePost(int postId);
        Task<bool> DislikePost(int postId);


    }
}