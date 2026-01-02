namespace Core;

public class ResponseDtoBase
{
    public ValidationResult Result { get; set; } = new();
}

public class ValidationResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}