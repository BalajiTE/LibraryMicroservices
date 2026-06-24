using Authors.Application.DTOs;
using Authors.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;

namespace Authors.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthorsController(IAuthorService authorService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuthorDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await authorService.GetAllAsync(cancellationToken));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var author = await authorService.GetByIdAsync(id, cancellationToken);
        return author is null ? NotFound() : Ok(author);
    }

    [Authorize(Roles = LibraryRoles.AdminOrLibrarian)]
    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Create(
        [FromBody] CreateAuthorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await authorService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = LibraryRoles.AdminOrLibrarian)]
    [HttpPut("{id}")]
    public async Task<ActionResult<AuthorDto>> Update(
        string id,
        [FromBody] UpdateAuthorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await authorService.UpdateAsync(id, request, cancellationToken);
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
        var deleted = await authorService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
