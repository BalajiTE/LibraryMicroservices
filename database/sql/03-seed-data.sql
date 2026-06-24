USE [LibraryMicroservices];
GO

DELETE FROM dbo.Loans;
DELETE FROM dbo.Books;
DELETE FROM dbo.Members;
DELETE FROM dbo.Authors;
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
