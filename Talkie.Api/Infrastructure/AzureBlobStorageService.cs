using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using Talkie.Api.Models;
using Microsoft.AspNetCore.ResponseCaching;
using Talkie.Domain.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Talkie.Api.Infrastructure
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("post-images");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<ResponseModel> UpsertPostImage(string userId, IFormFile image)
        {
            if (!(image.FileName.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) || image.FileName.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase)))
            {
                return new ResponseModel() { Status = "Error", Message = "Please, upload .jpg or .png image." };
            }
            if (image == null || image.Length <= 0)
            {
                return new ResponseModel() { Status = "Error", Message = "Invalid file!" };
            }

            var containerName = "post-images";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                await containerClient.CreateAsync();
                await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
            }

            var fileName = "post-" + userId + Path.GetExtension(image.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = image.OpenReadStream())
            {
                var compressedStream = CompressImage(stream);

                await blobClient.UploadAsync(compressedStream, true);
            }

            return new ResponseModel() { Status = "Success", Message = String.Concat("https://talkieapp.azurewebsites.net/api/Post/post-images/", fileName) };
        }
        public static Stream CompressImage(Stream stream)
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

    }




}

