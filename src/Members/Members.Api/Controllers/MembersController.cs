using Members.Application.DTOs;
using Members.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;

namespace Members.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MembersController(IMemberService memberService) : ControllerBase
{
    [Authorize(Roles = LibraryRoles.AnyAuthenticated)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await memberService.GetAllAsync(cancellationToken));

    [Authorize(Roles = LibraryRoles.AnyAuthenticated)]
    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var member = await memberService.GetByIdAsync(id, cancellationToken);
        return member is null ? NotFound() : Ok(member);
    }

    [Authorize(Roles = LibraryRoles.AdminOrLibrarian)]
    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create(
        [FromBody] CreateMemberRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await memberService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = LibraryRoles.AdminOrLibrarian)]
    [HttpPut("{id}")]
    public async Task<ActionResult<MemberDto>> Update(
        string id,
        [FromBody] UpdateMemberRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await memberService.UpdateAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = LibraryRoles.AdminOrLibrarian)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await memberService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
