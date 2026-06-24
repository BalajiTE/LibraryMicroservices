namespace Loans.Infrastructure.Options;

public class MembersApiOptions
{
    public const string SectionName = "MembersApi";

    public string BaseUrl { get; set; } = "http://localhost:5104";
}
