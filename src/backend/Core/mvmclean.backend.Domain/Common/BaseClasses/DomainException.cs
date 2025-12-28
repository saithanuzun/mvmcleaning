namespace mvmclean.backend.Domain.Common;


public abstract class DomainException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object> Metadata { get; }

    protected DomainException(string message, string errorCode) 
        : base(message)
    {
        ErrorCode = errorCode;
        Metadata = new Dictionary<string, object>();
    }

    protected DomainException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Metadata = new Dictionary<string, object>();
    }

    public void AddMetadata(string key, object value)
    {
        Metadata[key] = value;
    }
}