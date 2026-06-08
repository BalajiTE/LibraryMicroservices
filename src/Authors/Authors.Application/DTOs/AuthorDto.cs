namespace Authors.Application.DTOs;

public sealed record AuthorDto(string Id, string Name, string? Bio);

public sealed record CreateAuthorRequest(string Name, string? Bio);

public sealed record UpdateAuthorRequest(string Name, string? Bio);
