namespace mvmclean.backend.Domain.Core.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    string? DeletedBy { get; }
        
    void Delete(string? deletedBy = null);
    void Restore();

}