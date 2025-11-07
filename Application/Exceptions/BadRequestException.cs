namespace TipJar.Application.Exceptions;

public class BadRequestException(string message) : ApiException(400, message)
{}