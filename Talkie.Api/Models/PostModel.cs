using Talkie.Domain.Entities;

namespace Talkie.Api.Models
{
    public class PostModel
    {
        public int Id { get; set; }

        public string? Text { get; set; }
        public int LikeCount { get; set; }

        public DateTime PostedAt { get; set; }
        public string? ImageUrl { get; set; }

        public string AppUserId { get; set; }
    }
}
