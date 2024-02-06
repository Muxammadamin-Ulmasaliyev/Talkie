using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talkie.Domain.Entities
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Text { get; set; }
        public int LikeCount { get; set; } 

        public DateTime PostedAt { get; set; }
        public string? ImageUrl { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }



}
