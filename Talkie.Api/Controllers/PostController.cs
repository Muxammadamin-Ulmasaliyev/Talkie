using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talkie.Api.Models;
using Talkie.Api.Services;
using Talkie.Domain.Entities;

namespace Talkie.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly UserManager<AppUser> _userManager;
        private readonly BlobServiceClient _blobServiceClient;

        public PostController(IPostService postService, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _postService = postService;
            var connectionString = configuration.GetConnectionString("profile-images");

            _blobServiceClient = new BlobServiceClient(connectionString);

        }

        [AllowAnonymous]
        [Route("getAll")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostModel>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetAll());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{id:int:min(1)}")]
        [ProducesResponseType(typeof(PostModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 404)]

        public async Task<IActionResult> Get(int id)
        {
            var post = await _postService.Get(id);
            if (post != null)
            {
                return Ok(post);
            }
            return NotFound(new ResponseModel() { Status = "Error", Message = $"Post with id : {id} not found!" });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{username}/posts")]
        [ProducesResponseType(typeof(IEnumerable<PostModel>), 200)]
        public async Task<IActionResult> GetPostsByUsername(string username)
        {
            return Ok(await _postService.GetPostsByUsername(username));
        }


        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromForm] UpsertPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetCurrentUser();
            model.AppUserId = user.Id;



            var createdPost = await _postService.Create(model);
            // Increment user posts Count
            user.PostsCount++;
            var result = await _userManager.UpdateAsync(user);

            var routeValues = new { id = createdPost.Id };
            return CreatedAtRoute(routeValues, createdPost);
        }


       /* [HttpPatch]
        [Route("{id:int:min(1)}")]
        [ProducesResponseType(typeof(PostModel), 200)]
        public async Task<IActionResult> Update(int id, [FromForm] UpsertPostModel model)
        {
            var user = await GetCurrentUser();
            model.AppUserId = user.Id;
            var updatedPost = await _postService.Update(id, model);
            return Ok(updatedPost);
        }
       */
        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _postService.DeletePost(id);
            if (result)
            {
                return Ok(new ResponseModel() { Status = "Success", Message = $"Post with id : {id} deleted successfully!" });
            }
            return NotFound(new ResponseModel() { Status = "Error", Message = $"Post with id : {id} not found!" });

        }

        [AllowAnonymous]
        [HttpPatch]
        [Route("{postId:int:min(1)}/like")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        public async Task<IActionResult> LikePost(int postId)
        {
            var result = await _postService.LikePost(postId);
            if (result)
            {
                return Ok(new ResponseModel() { Status = "Success", Message = "Post liked successfully!" });
            }
            return NotFound(new ResponseModel() { Status = "Error", Message = $"Post with id : {postId} not found!" });
        }

        [AllowAnonymous]
        [HttpPatch]
        [Route("{postId:int:min(1)}/dislike")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        public async Task<IActionResult> DislikePost(int postId)
        {
            var result = await _postService.DislikePost(postId);
            if (result)
            {
                return Ok(new ResponseModel() { Status = "Success", Message = "Post disliked successfully!" });
            }
            return NotFound(new ResponseModel() { Status = "Error", Message = $"Post with id : {postId} not found!" });
        }

        [AllowAnonymous]
        [Route("post-images/{imageName}")]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(File), 200)]
        public async Task<IActionResult> GetPostImage(string imageName)
        {
            var containerName = "post-images";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(imageName);
            var imageStream = await blobClient.OpenReadAsync();

            if (imageStream == null)
                return NotFound(new ResponseModel() { Status = "Error", Message = "Image not found!" }); // Image not found

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



        [NonAction]
        public async Task<AppUser> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }

    }
}
