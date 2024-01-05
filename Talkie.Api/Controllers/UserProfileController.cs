using Asp.Versioning;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Talkie.Api.Models;
using Talkie.Api.Profiles;
using Talkie.Domain.Entities;


namespace Talkie.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

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
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(IdentityError), 400)]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<IActionResult> UpdateUserInfo([FromForm] UserProfileUpdateModel model)
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return NotFound(new ResponseModel() { Status = "Error", Message = "User not found!" });
            }
            if (model.FirstName != null) user.FirstName = model.FirstName;
            if (model.LastName != null) user.LastName = model.LastName;
            if (model.Bio != null) user.Bio = model.Bio;
            if (model.BirthDate != null) user.BirthDate = model.BirthDate;
            if (model.PhoneNumber != null) user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok(new ResponseModel() { Status = "Success", Message = "Profile updated successfully!" });
            else
                return BadRequest(result.Errors);


        }

        [Route("my-profile")]
        [HttpGet]
        [ProducesResponseType(typeof(UserProfileModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 404)]


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
                return NotFound(new ResponseModel() { Status = "Error", Message = "User not found!" });
            }

        }

        [AllowAnonymous]
        [Route("profile-images/{imageName}")]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(File), 200)]



        public async Task<IActionResult> GetProfileImage(string imageName)
        {
            // var user = await GetCurrentUser();
            var containerName = "profile-images";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(imageName);
            var imageStream = await blobClient.OpenReadAsync();

            if (imageStream == null)
                return NotFound(new ResponseModel() { Status = "Error", Message = "Image not found!" }); // Image not found

            // Set the content type based on the image file extension
            var contentType = GetContentTypeFromFileName(imageName);

            return File(imageStream, contentType);
        }

        [NonAction]
        private static string GetContentTypeFromFileName(string fileName)
        {
            if (fileName.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase))
                return "image/jpeg";
            if (fileName.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
                return "image/png";


            return "application/octet-stream"; // Default content type for binary data
        }

        [Route("update-profile-image")]
        [HttpPut]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(IdentityError), 400)]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<IActionResult> UpsertProfileImage([FromForm] IFormFile image)
        {
            if (!(image.FileName.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) || image.FileName.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new ResponseModel() { Status = "Error", Message = "Upload .jpg or .png image." });
            }
            var user = await GetCurrentUser();
            if (user == null)
            {
                return NotFound(new ResponseModel() { Status = "Error", Message = "User not found!" });

            }
            if (image == null || image.Length <= 0)
            {
                return BadRequest(new ResponseModel() { Status = "Error", Message = "Invalid file!" });
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
                // OLD VERSION
                //await blobClient.UploadAsync(stream, true);

                // NEW VERSION (compress images)
                var compressedStream = CompressImage(stream);

                await blobClient.UploadAsync(compressedStream, true);

            }

            user.ImageName = fileName;
            user.ProfileImageUrl = String.Concat("https://talkieapp.azurewebsites.net/api/UserProfile/profile-images/", user.ImageName);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new ResponseModel() { Status = "Success", Message = "Profile image updated successfully" });
            else
                return BadRequest(result.Errors);
        }

        [NonAction]
        public Stream CompressImage(Stream stream)
        {
            using (var image = Image.Load(stream))
            {
                image.Mutate(i => i.Resize(new ResizeOptions
                {
                    Size = new Size(800, 800),
                    Mode = ResizeMode.Max
                }));
                var compressedStream = new MemoryStream();
                image.Save(compressedStream, new JpegEncoder { Quality = 80 });
                compressedStream.Position = 0;
                return compressedStream;
            }
        }
        [Route("all-users")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AppUser>), 200)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }
        [NonAction]
        public async Task<AppUser> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}
