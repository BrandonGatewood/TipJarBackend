namespace TipJar.Application.Exceptions;

public class InvalidPasswordException(string message) : ApiException(400, message)
{}