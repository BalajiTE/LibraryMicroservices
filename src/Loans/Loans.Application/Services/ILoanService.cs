using Loans.Application.DTOs;

namespace Loans.Application.Services;

public interface ILoanService
{
    Task<IReadOnlyList<LoanDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LoanDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<LoanDto> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
    Task<LoanDto?> ReturnAsync(string id, ReturnLoanRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
