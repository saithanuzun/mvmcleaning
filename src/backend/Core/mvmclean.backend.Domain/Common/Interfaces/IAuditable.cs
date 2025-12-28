namespace mvmclean.backend.Domain.Common;

public interface IAuditable
{
    DateTime CreatedAt { get; }
    string? CreatedBy { get; }
    DateTime? UpdatedAt { get; }
    string? UpdatedBy { get; }
}