namespace mvmclean.backend.Domain.Common;

/// <summary>
/// Base class for all entities in the domain.
/// Entities have identity and lifecycle.
/// Includes audit tracking and soft delete functionality.
/// </summary>
public abstract class Entity : IEquatable<Entity>, IAuditable, ISoftDeletable
{
    private int? _requestedHashCode;

    public Guid Id { get; protected set; }
    
    // Audit properties
    public DateTime CreatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    
    // Soft delete properties
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    protected Entity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Check if entity is transient (not persisted yet)
    /// </summary>
    public bool IsTransient()
    {
        return Id == default;
    }

    /// <summary>
    /// Mark entity as updated with optional user tracking
    /// </summary>
    protected void MarkAsUpdated(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Soft delete the entity
    /// </summary>
    public void Delete(string? deletedBy = null)
    {
        if (IsDeleted)
            return; // Already deleted

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restore a soft-deleted entity
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted)
            return; // Not deleted

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    #region Equality

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (IsTransient() || other.IsTransient())
            return false;

        return Id == other.Id;
    }

    public bool Equals(Entity? other)
    {
        return Equals((object?)other);
    }

    public override int GetHashCode()
    {
        if (IsTransient())
            return base.GetHashCode();

        if (!_requestedHashCode.HasValue)
            _requestedHashCode = Id.GetHashCode() ^ 31;

        return _requestedHashCode.Value;
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }

    #endregion
}