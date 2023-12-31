using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Talkie.Api.Models;
using Talkie.Api.Profiles;
using Talkie.Domain.Entities;

namespace Talkie.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BlobServiceClient _blobServiceClient;
        public UserProfileController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;

            var connectionString = configuration.GetConnectionString("profile-images");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }


        [Route("update-user-profile")]
        [HttpPatch]
        public async Task<IActionResult> UpdateUserInfo([FromForm] UserProfileUpdateModel model)
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return NotFound("User not found!");
            }
            if (model.FirstName != null) user.FirstName = model.FirstName;
            if (model.LastName != null) user.LastName = model.LastName;
            if (model.Bio != null) user.Bio = model.Bio;
            if (model.BirthDate != null) user.BirthDate = model.BirthDate;
            if (model.PhoneNumber != null) user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok("Profile updated successfully!");
            else
                return BadRequest(result.Errors);


        }

        [Route("my-profile")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userFromDb = await GetCurrentUser();
            if (userFromDb != null)
            {
                var userModel = Mapper.Map(userFromDb);
                return Ok(userModel);
            }
            else
            {
                return NotFound("User not found");
            }

        }


        [Route("update-profile-image")]
        [HttpPut]
        public async Task<IActionResult> UpsertProfileImage([FromForm] IFormFile image)
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (image == null || image.Length <= 0)
            {
                return BadRequest("Invalid file");
            }
            var containerName = "profile-images";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                await containerClient.CreateAsync();
                await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
            }
            var fileName = user.Id + Path.GetExtension(image.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = image.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }
            var imageUrl = blobClient.Uri.ToString();
            user.ProfileImageUrl = imageUrl;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok("Profile image updated successfully");
            else
                return BadRequest(result.Errors);

        }
        [Route("all-users")]
        [HttpGet]
        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }
        [NonAction]
        public async Task<AppUser> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}
