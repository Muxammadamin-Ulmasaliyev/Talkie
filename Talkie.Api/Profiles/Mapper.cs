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
                ProfileImageUrl = appUser.ProfileImageUrl,
                ImageName = appUser.ImageName,
                PostsCount = appUser.PostsCount,
                Country = appUser.Country,
            };

        }

        public static Post Map(UpsertPostModel model)
        {
            return new Post()
            {
                Id = model.Id,
                Text = model.Text,
                LikeCount = 0,
                PostedAt = DateTime.Now,
                ImageUrl = model.Image == null ? string.Empty : model.ImageUrl,
                AppUserId = model.AppUserId
            };
        }
        public static Post Map(int id,UpsertPostModel model)
        {
            return new Post()
            {
                Id = id,
                Text = model.Text,
                LikeCount = 0,
                PostedAt = DateTime.Now,
                ImageUrl = model.Image == null ? string.Empty : model.ImageUrl,
                AppUserId = model.AppUserId
            };
        }

        public static PostModel Map(Post post)
        {
            return new PostModel()
            {
                Id = post.Id,
                Text = post.Text,
                LikeCount = post.LikeCount,
                PostedAt = post.PostedAt,
                ImageUrl = post.ImageUrl,
                AppUserId = post.AppUserId


            };
        }
       
    }
}
