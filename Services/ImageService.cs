using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using ShopAppBackend.Settings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;
using Flurl;
using ShopAppBackend.Settings.Interfaces;

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
            BlobClient blobClient = new BlobClient(_imageSettings.ConnectionString, _imageSettings.ContainerName, fileName);
            blobClient.DeleteIfExists();
        }

        public async Task<string> Uploader(IFormFile image, bool doCompress = true)
        {
            var fileName = Guid.NewGuid().ToString() + ".jpg";

            BlobClient blobClient = new BlobClient(_imageSettings.ConnectionString, _imageSettings.ContainerName, fileName);

            await using MemoryStream imageStream = new MemoryStream();
            await image.CopyToAsync(imageStream);

            await using MemoryStream output = new MemoryStream();
            await ConvertImg(imageStream, output, doCompress);

            await blobClient.UploadAsync(output, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" }
            });

            return fileName;
        }

        public async Task ConvertImg(Stream input, Stream output, bool doCompress)
        {
            input.Position = 0;

            using Image<Rgba32> image = await Image.LoadAsync<Rgba32>(input);

            if (doCompress)
            {
                image.Metadata.ExifProfile = null;

                double ratio = Convert.ToDouble(image.Width) / image.Height;

                if (image.Width >= image.Height && image.Width > _imageSettings.MaxWidth)
                {
                    image.Mutate(x => x
                        .Resize(_imageSettings.MaxWidth, (int)Math.Round(_imageSettings.MaxWidth / ratio))
                    );
                }
                else if (image.Width < image.Height && image.Height > _imageSettings.MaxWidth)
                {
                    image.Mutate(x => x
                        .Resize((int)Math.Round(_imageSettings.MaxWidth * ratio), _imageSettings.MaxWidth)
                    );
                }
            }

            await image.SaveAsJpegAsync(output);
            output.Position = 0;
            return;
        }
    }


}
