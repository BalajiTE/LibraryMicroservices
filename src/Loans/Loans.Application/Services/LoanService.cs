using Loans.Application.DTOs;
using Loans.Domain.Entities;
using Loans.Domain.Repositories;

namespace Loans.Application.Services;

public sealed class LoanService(ILoanRepository repository) : ILoanService
{
    public async Task<IReadOnlyList<LoanDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var loans = await repository.GetAllAsync(cancellationToken);
        return loans.Select(Map).ToList();
    }

    public async Task<LoanDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var loan = await repository.GetByIdAsync(id, cancellationToken);
        return loan is null ? null : Map(loan);
    }

    public async Task<LoanDto> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.BookId))
        {
            throw new ArgumentException("BookId is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.BorrowerName))
        {
            throw new ArgumentException("BorrowerName is required.", nameof(request));
        }

        var loan = new Loan
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            BookId = request.BookId.Trim(),
            BorrowerName = request.BorrowerName.Trim(),
            LoanDate = request.LoanDate,
            ReturnDate = null
        };

        var created = await repository.AddAsync(loan, cancellationToken);
        return Map(created);
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
                BorrowerName = existing.BorrowerName,
                LoanDate = existing.LoanDate,
                ReturnDate = request.ReturnDate
            },
            cancellationToken);

        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);

    private static LoanDto Map(Loan loan) =>
        new(loan.Id, loan.BookId, loan.BorrowerName, loan.LoanDate, loan.ReturnDate);
}
