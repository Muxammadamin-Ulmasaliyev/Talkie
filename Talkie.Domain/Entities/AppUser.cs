using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talkie.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? BirthDate { get; set; }
        public string? Country { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.Now;

        public string? ProfileImageUrl { get; set; } = string.Empty;
        public string? ImageName { get; set; } = string.Empty;

        public int PostsCount { get; set; } = 0;


        public List<Post> Posts { get; set; }


    }
}
