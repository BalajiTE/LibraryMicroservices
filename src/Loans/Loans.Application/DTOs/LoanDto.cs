namespace Loans.Application.DTOs;

public sealed record LoanDto(
    string Id,
    string BookId,
    string BorrowerName,
    DateOnly LoanDate,
    DateOnly? ReturnDate);

public sealed record CreateLoanRequest(
    string BookId,
    string BorrowerName,
    DateOnly LoanDate);

public sealed record ReturnLoanRequest(DateOnly ReturnDate);
