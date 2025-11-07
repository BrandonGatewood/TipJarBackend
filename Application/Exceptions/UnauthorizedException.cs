namespace TipJar.Application.Exceptions;

public class UnauthorizedException(string message) : ApiException(401, message)
{}