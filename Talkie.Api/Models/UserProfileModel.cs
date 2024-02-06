namespace Talkie.Api.Models
{
    public class UserProfileModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Country { get; set; }
        public string? Email { get; set; }

        public string? JoinedAt { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? ImageName { get; set; }
        public int? PostsCount { get; set; } 


    }
}
