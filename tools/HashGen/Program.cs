using Microsoft.AspNetCore.Identity;

var hasher = new PasswordHasher<object>();
Console.WriteLine(hasher.HashPassword(null!, "Password123!"));
