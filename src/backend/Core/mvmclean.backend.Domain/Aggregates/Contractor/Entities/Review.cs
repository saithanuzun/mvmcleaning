using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Contractor.Entities;

public class Review : Entity
{
    public Guid ContractorId { get; private set; }
    public Contractor Contractor { get; set; }
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
    {
        Id = id;
        SetRating(rating);
        SetComment(comment);

        ContractorId = contractorId;
        AuthorId = authorId;
    }

    public void Update(int rating, string comment)
    {
        SetRating(rating);
        SetComment(comment);
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