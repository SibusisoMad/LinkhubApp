namespace LinkHub.UI.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool Success { get; set; }
        public string? Error { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public static ErrorViewModel Ok() => new() { Success = true };

        public static ErrorViewModel Fail(string error) =>
            new() { Success = false, Error = error };
        
    }

    public class ApiResponse<T> : ErrorViewModel
    {
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data) =>
            new() { Success = true, Data = data };

        public new static ApiResponse<T> Fail(string error) =>
            new() { Success = false, Error = error };
    }
}
