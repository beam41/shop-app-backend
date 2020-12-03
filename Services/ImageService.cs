using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Flurl;
using Microsoft.AspNetCore.Http;
using ShopAppBackend.Settings.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ShopAppBackend.Services
{
    public class ImageService
    {
        private readonly IImageSettings _imageSettings;

        public ImageService(IImageSettings imageSettings)
        {
            _imageSettings = imageSettings;
        }

        public string GetImageUrl(string fileName)
        {
            return Url.Combine(_imageSettings.BlobPath, _imageSettings.ContainerName, fileName);
        }

        public void DeleteFile(string fileName)
        {
            var blobClient = new BlobClient(_imageSettings.ConnectionString, _imageSettings.ContainerName, fileName);
            blobClient.DeleteIfExists();
        }

        public async Task<string> Uploader(IFormFile image, bool doCompress = true)
        {
            var fileName = Guid.NewGuid() + ".jpg";

            var blobClient = new BlobClient(_imageSettings.ConnectionString, _imageSettings.ContainerName, fileName);

            await using var imageStream = new MemoryStream();
            await image.CopyToAsync(imageStream);

            await using var output = new MemoryStream();
            await ConvertImg(imageStream, output, doCompress);

            await blobClient.UploadAsync(output, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" }
            });

            return fileName;
        }

        private async Task ConvertImg(Stream input, Stream output, bool doCompress)
        {
            input.Position = 0;

            using var image = await Image.LoadAsync<Rgba32>(input);

            if (doCompress)
            {
                image.Metadata.ExifProfile = null;

                var ratio = Convert.ToDouble(image.Width) / image.Height;

                if (image.Width >= image.Height && image.Width > _imageSettings.MaxWidth)
                    image.Mutate(x => x
                        .Resize(_imageSettings.MaxWidth, (int) Math.Round(_imageSettings.MaxWidth / ratio))
                    );
                else if (image.Width < image.Height && image.Height > _imageSettings.MaxWidth)
                    image.Mutate(x => x
                        .Resize((int) Math.Round(_imageSettings.MaxWidth * ratio), _imageSettings.MaxWidth)
                    );
            }

            await image.SaveAsJpegAsync(output);
            output.Position = 0;
        }
    }
}
