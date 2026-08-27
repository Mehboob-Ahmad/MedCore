namespace MedicHp.Shared.Exceptions;

public class BadRequestException : BaseException
{
    public BadRequestException(string message) 
        : base(message, "BAD_REQUEST")
    {
    }

    public BadRequestException(string errorCode, string message) 
        : base(message, errorCode)
    {
    }
}
