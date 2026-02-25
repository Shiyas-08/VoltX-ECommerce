    namespace ECommerce.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string msg, int statusCode)
            => new()
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Data = data,
                Message = msg
            };

        public static ApiResponse<string> Fail(string msg, int statusCode)
            => new()
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = msg
            };
    }

}

