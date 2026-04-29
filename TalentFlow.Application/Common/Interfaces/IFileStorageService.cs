using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string container);
        Task<string> SaveFileAsync(byte[] content, string fileName, string container);
        Task<bool> DeleteFileAsync(string fileUrl);
    }
}
