using Auth.Application.DTOs;
using Auth.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = LibraryRoles.Admin)]
public sealed class RolesController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await roleService.GetAllAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var role = await roleService.GetByIdAsync(id, cancellationToken);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await roleService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> Update(
        string id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await roleService.UpdateAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await roleService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
