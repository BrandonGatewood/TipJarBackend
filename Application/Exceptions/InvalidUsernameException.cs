namespace TipJar.Application.Exceptions;

public class InvalidUsernameException(string message) : ApiException(400, message)
{}