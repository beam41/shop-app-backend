using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using ShopAppBackend.Settings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Services
{
    public class ImageService
    {
        private readonly IImageSettings _settings;

        public ImageService(IImageSettings settings)
        {
            _settings = settings;
        }

        public async Task<string> Uploader(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + ".jpg";

            BlobClient blobClient = new BlobClient(_settings.ConnectionString, _settings.ContainerName, fileName);

            using MemoryStream imageStream = new MemoryStream();
            await image.CopyToAsync(imageStream);

            using MemoryStream output = new MemoryStream();
            await ConvertImg(imageStream, output);

            await blobClient.UploadAsync(output, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" }
                }
            );

            return fileName;
        }

        public async Task ConvertImg(Stream input, Stream output)
        {
            input.Position = 0;

            using Image<Rgba32> image = await Image.LoadAsync<Rgba32>(input);
            image.Metadata.ExifProfile = null;

            double ratio = Convert.ToDouble(image.Width) / image.Height;

            if (image.Width >= image.Height && image.Width > _settings.MaxWidth)
            {
                image.Mutate(x => x
                    .Resize(_settings.MaxWidth, (int)Math.Round(_settings.MaxWidth / ratio))
                );
            }
            else if (image.Width < image.Height && image.Height > _settings.MaxWidth)
            {
                image.Mutate(x => x
                    .Resize((int)Math.Round(_settings.MaxWidth * ratio), _settings.MaxWidth)
                );
            }

            await image.SaveAsJpegAsync(output);
            output.Position = 0;
            return;
        }
    }


}
