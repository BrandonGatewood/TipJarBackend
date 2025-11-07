namespace TipJar.Application.Exceptions;

public class NotFoundException(string message) : ApiException(404, message)
{}