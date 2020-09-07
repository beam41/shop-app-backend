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

        public async Task<bool> ConvertImg(Stream input, Stream output)
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
            return true;
        }
    }


}
