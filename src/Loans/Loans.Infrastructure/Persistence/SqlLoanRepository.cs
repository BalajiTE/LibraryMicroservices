using Loans.Domain.Entities;
using Loans.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Persistence;
using Shared.Persistence.Entities;

namespace Loans.Infrastructure.Persistence;

public sealed class SqlLoanRepository(LibraryDbContext context) : ILoanRepository
{
    public async Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var loans = await context.Loans
            .AsNoTracking()
            .OrderByDescending(loan => loan.LoanDate)
            .ToListAsync(cancellationToken);

        return loans.Select(Map).ToList();
    }

    public async Task<Loan?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var loan = await context.Loans
            .AsNoTracking()
            .FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

        return loan is null ? null : Map(loan);
    }

    public async Task<Loan> AddAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        context.Loans.Add(Map(loan));
        await context.SaveChangesAsync(cancellationToken);
        return loan;
    }

    public async Task<Loan?> UpdateAsync(string id, Loan loan, CancellationToken cancellationToken = default)
    {
        var existing = await context.Loans.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.BookId = loan.BookId;
        existing.MemberId = loan.MemberId;
        existing.LoanDate = loan.LoanDate;
        existing.ReturnDate = loan.ReturnDate;
        await context.SaveChangesAsync(cancellationToken);
        return Map(existing);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var existing = await context.Loans.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        context.Loans.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Loan Map(LoanRecord record) =>
        new()
        {
            Id = record.Id,
            BookId = record.BookId,
            MemberId = record.MemberId,
            LoanDate = record.LoanDate,
            ReturnDate = record.ReturnDate
        };

    private static LoanRecord Map(Loan loan) =>
        new()
        {
            Id = loan.Id,
            BookId = loan.BookId,
            MemberId = loan.MemberId,
            LoanDate = loan.LoanDate,
            ReturnDate = loan.ReturnDate
        };
}
