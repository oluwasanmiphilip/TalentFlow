namespace TalentFlow.Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public object? Errors { get; set; }

        // Success factory
        public static ApiResponse<T> SuccessResponse(T data, string message = "", int statusCode = 200) =>
            new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode,
                Errors = null
            };

        // Fail factory with optional errors
        public static ApiResponse<T> Fail(string message, int statusCode = 400, object? errors = null) =>
            new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
    }

    // Non-generic convenience wrapper for controllers that prefer ApiResponse<object>
    public static class ApiResponse
    {
        public static ApiResponse<T> Success<T>(T data, string message = "", int statusCode = 200) =>
            ApiResponse<T>.SuccessResponse(data, message, statusCode);

        public static ApiResponse<T> Fail<T>(string message, int statusCode = 400, object? errors = null) =>
            ApiResponse<T>.Fail(message, statusCode, errors);
    }
}
