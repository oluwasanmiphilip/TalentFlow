//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Options;
//using System;
//using System.IO;
//using System.Threading.Tasks;
//using TalentFlow.Application.Common.Interfaces;

//public class LocalFileStorageService : IFileStorageService
//{
//    private readonly IWebHostEnvironment _env;
//    private readonly FileStorageOptions _options;

//    public LocalFileStorageService(IWebHostEnvironment env, IOptions<FileStorageOptions> options)
//    {
//        _env = env;
//        _options = options.Value;
//    }

//    public async Task<string> SaveFileAsync(IFormFile file, string container)
//    {
//        var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), _options.UploadsPath);
//        var containerPath = Path.Combine(uploadsRoot, container);
//        Directory.CreateDirectory(containerPath);

//        var ext = Path.GetExtension(file.FileName);
//        var fileName = $"{Guid.NewGuid():N}{ext}";
//        var filePath = Path.Combine(containerPath, fileName);

//        using (var stream = new FileStream(filePath, FileMode.Create))
//        {
//            await file.CopyToAsync(stream);
//        }

//        var relative = $"/{_options.UploadsPath}/{container}/{fileName}";
//        return $"{_options.BaseUrl.TrimEnd('/')}{relative}";
//    }

//    public async Task<string> SaveFileAsync(byte[] content, string fileName, string container)
//    {
//        var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), _options.UploadsPath);
//        var containerPath = Path.Combine(uploadsRoot, container);
//        Directory.CreateDirectory(containerPath);

//        var safeName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
//        var filePath = Path.Combine(containerPath, safeName);

//        await File.WriteAllBytesAsync(filePath, content);

//        var relative = $"/{_options.UploadsPath}/{container}/{safeName}";
//        return $"{_options.BaseUrl.TrimEnd('/')}{relative}";
//    }

//    public Task<bool> DeleteFileAsync(string fileUrl)
//    {
//        if (string.IsNullOrWhiteSpace(fileUrl)) return Task.FromResult(false);

//        // Convert absolute URL to local path
//        var relative = fileUrl.Replace(_options.BaseUrl.TrimEnd('/'), "").TrimStart('/');
//        var filePath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), relative.Replace('/', Path.DirectorySeparatorChar));

//        if (!File.Exists(filePath)) return Task.FromResult(false);

//        File.Delete(filePath);
//        return Task.FromResult(true);
//    }
//}
