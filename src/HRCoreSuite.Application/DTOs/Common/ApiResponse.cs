using System.Text.Json.Serialization;

namespace HRCoreSuite.Application.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; } 
    
    public T? Data { get; }
    public object? Errors { get; }

    [JsonConstructor]
    public ApiResponse(bool success, T? data, object? errors) 
    {
        Success = success;
        Data = data;
        Errors = errors;
    }
    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>(true, data, null);
    }

    public static ApiResponse<object> FailResponse(object? errors)
    {
        return new ApiResponse<object>(false, null, errors?? new object());
    }
}