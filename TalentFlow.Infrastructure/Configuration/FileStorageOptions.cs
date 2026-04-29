namespace TalentFlow.Infrastructure.Configuration
{
    public class FileStorageOptions
    {
        /// <summary>
        /// Public base URL used to build returned file URLs (e.g. https://api.example.com).
        /// </summary>
        public string BaseUrl { get; set; } = "http://localhost:5000";

        /// <summary>
        /// Relative uploads folder under wwwroot (e.g. "uploads").
        /// </summary>
        public string UploadsPath { get; set; } = "uploads";
    }
}
