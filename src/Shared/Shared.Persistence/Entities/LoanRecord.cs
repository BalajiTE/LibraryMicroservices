namespace Shared.Persistence.Entities;

public sealed class LoanRecord
{
    public string Id { get; set; } = null!;
    public string BookId { get; set; } = null!;
    public string MemberId { get; set; } = null!;
    public DateOnly LoanDate { get; set; }
    public DateOnly? ReturnDate { get; set; }
}
