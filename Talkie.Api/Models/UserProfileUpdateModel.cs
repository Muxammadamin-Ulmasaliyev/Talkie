namespace Talkie.Api.Models
{
    public class UserProfileUpdateModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Country { get; set; }
    }
}
