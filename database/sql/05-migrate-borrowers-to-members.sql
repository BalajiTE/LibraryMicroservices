USE [LibraryMicroservices];
GO

IF OBJECT_ID(N'dbo.Loans', N'U') IS NOT NULL
    DROP TABLE dbo.Loans;
GO

IF OBJECT_ID(N'dbo.Borrowers', N'U') IS NOT NULL
    DROP TABLE dbo.Borrowers;
GO

IF OBJECT_ID(N'dbo.Members', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Members
    (
        Id    NVARCHAR(50)  NOT NULL CONSTRAINT PK_Members PRIMARY KEY,
        Name  NVARCHAR(200) NOT NULL,
        Email NVARCHAR(200) NULL
    );
END
GO

CREATE TABLE dbo.Loans
(
    Id         NVARCHAR(50) NOT NULL CONSTRAINT PK_Loans PRIMARY KEY,
    BookId     NVARCHAR(50) NOT NULL,
    MemberId   NVARCHAR(50) NOT NULL,
    LoanDate   DATE         NOT NULL,
    ReturnDate DATE         NULL,
    CONSTRAINT FK_Loans_Books FOREIGN KEY (BookId) REFERENCES dbo.Books (Id),
    CONSTRAINT FK_Loans_Members FOREIGN KEY (MemberId) REFERENCES dbo.Members (Id)
);
GO

CREATE INDEX IX_Loans_BookId ON dbo.Loans (BookId);
CREATE INDEX IX_Loans_MemberId ON dbo.Loans (MemberId);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Members)
BEGIN
    INSERT INTO dbo.Members (Id, Name, Email)
    VALUES
        (N'm1', N'Alice Smith', N'alice.smith@example.com'),
        (N'm2', N'Bob Jones', N'bob.jones@example.com');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Loans)
BEGIN
    INSERT INTO dbo.Loans (Id, BookId, MemberId, LoanDate, ReturnDate)
    VALUES
        (N'l1', N'b1', N'm1', '2026-05-01', NULL),
        (N'l2', N'b2', N'm2', '2026-04-15', '2026-05-10');
END
GO
