using Loans.Application.DTOs;
using Loans.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loans.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LoansController(ILoanService loanService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LoanDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await loanService.GetAllAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var loan = await loanService.GetByIdAsync(id, cancellationToken);
        return loan is null ? NotFound() : Ok(loan);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create(
        [FromBody] CreateLoanRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await loanService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<LoanDto>> Return(
        string id,
        [FromBody] ReturnLoanRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await loanService.ReturnAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await loanService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
