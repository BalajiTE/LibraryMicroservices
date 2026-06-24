using Loans.Application.DTOs;
using Loans.Application.Integrations;
using Loans.Domain.Entities;
using Loans.Domain.Repositories;

namespace Loans.Application.Services;

public sealed class LoanService(
    ILoanRepository repository,
    IMembersApiClient membersApiClient) : ILoanService
{
    public async Task<IReadOnlyList<LoanDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var loans = await repository.GetAllAsync(cancellationToken);
        var memberNames = await membersApiClient.GetNameLookupAsync(cancellationToken);
        return loans.Select(loan => Map(loan, memberNames)).ToList();
    }

    public async Task<LoanDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var loan = await repository.GetByIdAsync(id, cancellationToken);
        if (loan is null)
        {
            return null;
        }

        var member = await membersApiClient.GetByIdAsync(loan.MemberId, cancellationToken);
        return Map(loan, member?.Name);
    }

    public async Task<LoanDto> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.BookId))
        {
            throw new ArgumentException("BookId is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.MemberId))
        {
            throw new ArgumentException("MemberId is required.", nameof(request));
        }

        var member = await membersApiClient.GetByIdAsync(request.MemberId.Trim(), cancellationToken);
        if (member is null)
        {
            throw new ArgumentException($"Member '{request.MemberId}' was not found.", nameof(request));
        }

        var loan = new Loan
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            BookId = request.BookId.Trim(),
            MemberId = member.Id,
            LoanDate = request.LoanDate,
            ReturnDate = null
        };

        var created = await repository.AddAsync(loan, cancellationToken);
        return Map(created, member.Name);
    }

    public async Task<LoanDto?> ReturnAsync(string id, ReturnLoanRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        if (existing.ReturnDate is not null)
        {
            throw new InvalidOperationException("Loan has already been returned.");
        }

        if (request.ReturnDate < existing.LoanDate)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "ReturnDate cannot be before LoanDate.");
        }

        var updated = await repository.UpdateAsync(
            id,
            new Loan
            {
                Id = existing.Id,
                BookId = existing.BookId,
                MemberId = existing.MemberId,
                LoanDate = existing.LoanDate,
                ReturnDate = request.ReturnDate
            },
            cancellationToken);

        if (updated is null)
        {
            return null;
        }

        var member = await membersApiClient.GetByIdAsync(updated.MemberId, cancellationToken);
        return Map(updated, member?.Name);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);

    private static LoanDto Map(Loan loan, IReadOnlyDictionary<string, string> memberNames) =>
        Map(loan, memberNames.GetValueOrDefault(loan.MemberId, "Unknown"));

    private static LoanDto Map(Loan loan, string? memberName) =>
        new(loan.Id, loan.BookId, loan.MemberId, memberName ?? "Unknown", loan.LoanDate, loan.ReturnDate);
}
