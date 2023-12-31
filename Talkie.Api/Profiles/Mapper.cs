using Talkie.Api.Models;
using Talkie.Domain.Entities;

namespace Talkie.Api.Profiles
{
    public static class Mapper
    {
        public static UserProfileModel Map(AppUser appUser)
        {
            return new UserProfileModel()
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Bio = appUser.Bio,
                BirthDate = appUser.BirthDate,
                PhoneNumber = appUser.PhoneNumber,
                Email = appUser.Email,
                JoinedAt = appUser.JoinedAt.ToShortDateString(),
                ProfileImageUrl = appUser.ProfileImageUrl


            };

        }
    }
}
