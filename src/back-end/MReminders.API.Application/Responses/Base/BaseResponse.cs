namespace MReminders.API.Application.Responses.Base;

public class BaseResponse<T>
{
    public T Data { get; set; } = default!;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
