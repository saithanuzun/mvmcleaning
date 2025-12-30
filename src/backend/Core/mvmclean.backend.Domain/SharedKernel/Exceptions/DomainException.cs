namespace mvmclean.backend.Domain.SharedKernel.Exceptions;

public class DomainException : Core.BaseClasses.DomainException
{
    public DomainException(string message):base(message){}
    public DomainException(string message, string errorCode) : base(message, errorCode)
    {
    }

    public DomainException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }
}