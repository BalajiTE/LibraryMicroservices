using Loans.Domain.Entities;
using Loans.Domain.Repositories;
using Shared.BuildingBlocks.Persistence;

namespace Loans.Infrastructure.Persistence;

public sealed class JsonLoanRepository : JsonFileRepository<Loan, string>, ILoanRepository
{
    public JsonLoanRepository(string filePath) : base(filePath)
    {
    }

    protected override string GetKey(Loan entity) => entity.Id;

    protected override Loan WithKey(Loan entity, string key) =>
        new()
        {
            Id = key,
            BookId = entity.BookId,
            BorrowerName = entity.BorrowerName,
            LoanDate = entity.LoanDate,
            ReturnDate = entity.ReturnDate
        };

    public Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default) =>
        base.GetAllAsync(cancellationToken);

    public Task<Loan?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        base.GetByIdAsync(id, cancellationToken);

    public Task<Loan> AddAsync(Loan loan, CancellationToken cancellationToken = default) =>
        base.AddAsync(loan, cancellationToken);

    public Task<Loan?> UpdateAsync(string id, Loan loan, CancellationToken cancellationToken = default) =>
        base.UpdateAsync(
            id,
            _ => new Loan
            {
                Id = id,
                BookId = loan.BookId,
                BorrowerName = loan.BorrowerName,
                LoanDate = loan.LoanDate,
                ReturnDate = loan.ReturnDate
            },
            cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        base.DeleteAsync(id, cancellationToken);
}
