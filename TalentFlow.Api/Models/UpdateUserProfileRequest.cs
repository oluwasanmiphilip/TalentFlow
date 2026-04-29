using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TalentFlow.Api.Models
{
    public class UpdateUserProfileRequest
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        public bool? EmailNotifications { get; set; }

        public IFormFile? ProfilePhoto { get; set; }

        [Url]
        public string? ProfilePhotoUrl { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
