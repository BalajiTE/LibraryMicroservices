using Loans.Domain.Entities;

namespace Loans.Domain.Repositories;

public interface ILoanRepository
{
    Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Loan?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Loan> AddAsync(Loan loan, CancellationToken cancellationToken = default);
    Task<Loan?> UpdateAsync(string id, Loan loan, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
