using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;

namespace TalentFlow.Infrastructure.Services
{
    public static class ImageProcessingHelper
    {
        public static async Task<(byte[] imageBytes, byte[] thumbnailBytes)> ProcessImageAsync(IFormFile file, int maxDimension = 1024, int thumbSize = 200)
        {
            using var input = file.OpenReadStream();
            using var image = await Image.LoadAsync(input);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxDimension, maxDimension)
            }));

            var jpegEncoder = new JpegEncoder { Quality = 85 };

            using var ms = new MemoryStream();
            await image.SaveAsJpegAsync(ms, jpegEncoder);
            var imageBytes = ms.ToArray();

            // Create thumbnail from original stream again
            using var input2 = file.OpenReadStream();
            using var thumbImage = await Image.LoadAsync(input2);
            thumbImage.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size(thumbSize, thumbSize)
            }));

            using var msThumb = new MemoryStream();
            await thumbImage.SaveAsJpegAsync(msThumb, jpegEncoder);
            var thumbBytes = msThumb.ToArray();

            return (imageBytes, thumbBytes);
        }
    }
}
