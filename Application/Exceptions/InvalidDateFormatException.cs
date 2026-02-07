namespace TipJar.Application.Exceptions;

public class InvalidDateFormatException(string message) : ApiException(400, message)
{}