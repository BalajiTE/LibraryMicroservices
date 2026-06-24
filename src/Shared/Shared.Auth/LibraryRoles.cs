namespace Shared.Auth;

public static class LibraryRoles
{
    public const string Admin = "Admin";
    public const string Librarian = "Librarian";
    public const string Member = "Member";

    public const string AdminOrLibrarian = $"{Admin},{Librarian}";
    public const string AnyAuthenticated = $"{Admin},{Librarian},{Member}";
}
