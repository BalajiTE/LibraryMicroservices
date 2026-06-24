USE [LibraryMicroservices];
GO

DELETE FROM dbo.Loans;
DELETE FROM dbo.Books;
DELETE FROM dbo.Members;
DELETE FROM dbo.Authors;

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NOT NULL
    DELETE FROM dbo.UserRoles;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
    DELETE FROM dbo.Users;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL
    DELETE FROM dbo.Roles;
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.Roles (Id, Name, Description)
    VALUES
        (N'r1', N'Admin', N'Full system access including user management'),
        (N'r2', N'Librarian', N'Manage catalog, members, and loans'),
        (N'r3', N'Member', N'Browse catalog and view loans');
END
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.Users (Id, Username, Email, PasswordHash, IsActive)
    VALUES
        (N'u1', N'admin', N'admin@library.local', N'AQAAAAIAAYagAAAAEHJUDbb3QN71tMQBNybl/mCkDvozC17kDjEgowMcj/8w+eMehO90wICy28A5m8wgxg==', 1),
        (N'u2', N'librarian', N'librarian@library.local', N'AQAAAAIAAYagAAAAEHJUDbb3QN71tMQBNybl/mCkDvozC17kDjEgowMcj/8w+eMehO90wICy28A5m8wgxg==', 1),
        (N'u3', N'member', N'member@library.local', N'AQAAAAIAAYagAAAAEHJUDbb3QN71tMQBNybl/mCkDvozC17kDjEgowMcj/8w+eMehO90wICy28A5m8wgxg==', 1);
END
GO

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId)
    VALUES
        (N'u1', N'r1'),
        (N'u2', N'r2'),
        (N'u3', N'r3');
END
GO

INSERT INTO dbo.Authors (Id, Name, Bio)
VALUES
    (N'a1', N'Jane Austen', N'English novelist known for social commentary and romance.'),
    (N'a2', N'George Orwell', N'English novelist and essayist, author of dystopian fiction.'),
    (N'a3', N'Agatha Christie', N'Prolific writer of detective novels and short stories.');
GO

INSERT INTO dbo.Books (Id, Title, AuthorId, Isbn, PublishedYear)
VALUES
    (N'b1', N'Pride and Prejudice', N'a1', N'978-0141439518', 1813),
    (N'b2', N'1984', N'a2', N'978-0451524935', 1949),
    (N'b3', N'Murder on the Orient Express', N'a3', N'978-0062693662', 1934);
GO

INSERT INTO dbo.Members (Id, Name, Email)
VALUES
    (N'm1', N'Alice Smith', N'alice.smith@example.com'),
    (N'm2', N'Bob Jones', N'bob.jones@example.com');
GO

INSERT INTO dbo.Loans (Id, BookId, MemberId, LoanDate, ReturnDate)
VALUES
    (N'l1', N'b1', N'm1', '2026-05-01', NULL),
    (N'l2', N'b2', N'm2', '2026-04-15', '2026-05-10');
GO
