using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopAppBackend.Models;
using ShopAppBackend.Services;
using ShopAppBackend.Settings;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageSettings _settings;

        private readonly ImageService _imageService;

        public ImagesController(IImageSettings settings, ImageService imageService)
        {
            _settings = settings;
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage([FromForm] FormImage formImage)
        {
            var imageType = new string[] { "image/jpeg", "image/jpg", "image/png" };
            if (!imageType.Contains(formImage.Data.ContentType))
            {
                return BadRequest();
            }

            BlobClient blobClient = new BlobClient(_settings.ConnectionString, _settings.ContainerName, Guid.NewGuid().ToString() + ".jpg");

            using MemoryStream imageStream = new MemoryStream();
            await formImage.Data.CopyToAsync(imageStream);

            using MemoryStream output = new MemoryStream();

            await _imageService.ConvertImg(imageStream, output);

            await blobClient.UploadAsync(output);

            return Ok();
        }
    }
}
