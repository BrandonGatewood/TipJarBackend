namespace TipJar.Application.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; set; }

    protected ApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}