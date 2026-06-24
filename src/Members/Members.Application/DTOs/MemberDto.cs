namespace Members.Application.DTOs;

public sealed record MemberDto(string Id, string Name, string? Email);

public sealed record CreateMemberRequest(string Name, string? Email);

public sealed record UpdateMemberRequest(string Name, string? Email);
