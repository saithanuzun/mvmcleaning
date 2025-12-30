namespace mvmclean.backend.Domain.Core.BaseClasses;


public abstract class DomainException : Exception
{
    public string? ErrorCode { get; }
    public IReadOnlyDictionary<string, object> Metadata => _metadata;

    private readonly Dictionary<string, object> _metadata;

    protected DomainException(string message)
        : base(message)
    {
        _metadata = new Dictionary<string, object>();
    }

    protected DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
        _metadata = new Dictionary<string, object>();
    }

    protected DomainException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        _metadata = new Dictionary<string, object>();
    }

    protected void AddMetadata(string key, object value)
    {
        _metadata[key] = value;
    }
}
