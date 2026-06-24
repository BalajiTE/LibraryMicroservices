namespace Loans.Application.DTOs;

public sealed record LoanDto(
    string Id,
    string BookId,
    string MemberId,
    string MemberName,
    DateOnly LoanDate,
    DateOnly? ReturnDate);

public sealed record CreateLoanRequest(
    string BookId,
    string MemberId,
    DateOnly LoanDate);

public sealed record ReturnLoanRequest(DateOnly ReturnDate);
