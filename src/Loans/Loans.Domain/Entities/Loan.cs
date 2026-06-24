namespace Loans.Domain.Entities;

public sealed class Loan
{
    public required string Id { get; init; }
    public required string BookId { get; init; }
    public required string MemberId { get; init; }
    public required DateOnly LoanDate { get; init; }
    public DateOnly? ReturnDate { get; init; }
}
