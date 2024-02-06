using System.ComponentModel.DataAnnotations;
using Talkie.Domain.Entities;

namespace Talkie.Api.Models
{
    public class UpsertPostModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Text is required!")]
        public string Text { get; set; }

        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;

        public string? AppUserId { get; set; }
    }
}
