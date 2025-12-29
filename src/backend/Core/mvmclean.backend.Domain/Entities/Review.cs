using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Entities;

public class Review : Entity
{
    public Guid ContractorId { get; private set; }
    public Guid AuthorId { get; private set; }

    public int Rating { get; private set; }
    public string Comment { get; private set; }

    // EF Core constructor
    private Review() { }

    public Review(
        Guid id,
        Guid contractorId,
        Guid authorId,
        int rating,
        string comment)
        : base(id)
    {
        SetRating(rating);
        SetComment(comment);

        ContractorId = contractorId;
        AuthorId = authorId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(int rating, string comment)
    {
        SetRating(rating);
        SetComment(comment);
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new InvalidOperationException("Rating must be between 1 and 5.");

        Rating = rating;
    }

    private void SetComment(string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException("Comment cannot be empty.");

        if (comment.Length > 1000)
            throw new InvalidOperationException("Comment is too long.");

        Comment = comment;
    }
}
