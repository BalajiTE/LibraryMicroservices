using Books.Application.DTOs;
using Books.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BooksController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await bookService.GetAllAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var book = await bookService.GetByIdAsync(id, cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(
        [FromBody] CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await bookService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> Update(
        string id,
        [FromBody] UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await bookService.UpdateAsync(id, request, cancellationToken);
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
        var deleted = await bookService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
