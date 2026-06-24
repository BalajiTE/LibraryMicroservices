namespace Shared.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "LibraryMicroservices";
    public string Audience { get; set; } = "LibraryMicroservices";
    public string SecretKey { get; set; } = "LibraryMicroservices-Dev-Secret-Key-Min-32-Chars!";
    public int ExpirationMinutes { get; set; } = 60;
}
