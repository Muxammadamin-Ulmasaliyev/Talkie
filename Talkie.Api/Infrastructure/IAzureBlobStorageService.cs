using Talkie.Api.Models;
using Talkie.Domain.Entities;

namespace Talkie.Api.Infrastructure
{
    public interface IAzureBlobStorageService
    {
        public Task<ResponseModel> UpsertPostImage(string userId, IFormFile image);
    }
}
