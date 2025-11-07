namespace TipJar.Application.Exceptions;

public class ConflictException(string message) : ApiException(409, message)
{
}