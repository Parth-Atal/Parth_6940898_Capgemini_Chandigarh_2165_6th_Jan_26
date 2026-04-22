namespace BookStore.Shared;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message };
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }

    public static ApiResponse Ok(string message = "Success")
        => new() { Success = true, Message = message, StatusCode = 200 };

    public static ApiResponse Fail(string message, int code = 400)
        => new() { Success = false, Message = message, StatusCode = code };
}