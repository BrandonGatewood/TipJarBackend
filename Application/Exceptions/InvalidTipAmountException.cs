namespace TipJar.Application.Exceptions;

public class InvalidTipAmountException(string message) : ApiException(400, message)
{}