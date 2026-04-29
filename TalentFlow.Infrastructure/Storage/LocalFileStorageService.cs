using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Infrastructure.Configuration;

namespace TalentFlow.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IHostEnvironment _env;
        private readonly FileStorageOptions _options;

        public LocalFileStorageService(IHostEnvironment env, IOptions<FileStorageOptions> options)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        private string GetWebRootPath()
        {
            // IHostEnvironment does not expose WebRootPath; assume wwwroot under content root
            return Path.Combine(_env.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot");
        }

        public async Task<string> SaveFileAsync(IFormFile file, string container)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            var uploadsRoot = Path.Combine(GetWebRootPath(), _options.UploadsPath);
            var containerPath = Path.Combine(uploadsRoot, container);
            Directory.CreateDirectory(containerPath);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(containerPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
            }

            var relative = $"/{_options.UploadsPath}/{container}/{fileName}";
            return $"{_options.BaseUrl.TrimEnd('/')}{relative}";
        }

        public async Task<string> SaveFileAsync(byte[] content, string fileName, string container)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var uploadsRoot = Path.Combine(GetWebRootPath(), _options.UploadsPath);
            var containerPath = Path.Combine(uploadsRoot, container);
            Directory.CreateDirectory(containerPath);

            var safeName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(containerPath, safeName);

            await File.WriteAllBytesAsync(filePath, content);

            var relative = $"/{_options.UploadsPath}/{container}/{safeName}";
            return $"{_options.BaseUrl.TrimEnd('/')}{relative}";
        }

        public Task<bool> DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl)) return Task.FromResult(false);

            var baseUrlTrimmed = _options.BaseUrl?.TrimEnd('/') ?? string.Empty;
            var relative = fileUrl.Replace(baseUrlTrimmed, "").TrimStart('/');
            var filePath = Path.Combine(GetWebRootPath(), relative.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(filePath)) return Task.FromResult(false);

            File.Delete(filePath);
            return Task.FromResult(true);
        }
    }
}
