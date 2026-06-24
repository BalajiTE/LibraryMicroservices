USE [LibraryMicroservices];
GO

IF OBJECT_ID(N'dbo.Loans', N'U') IS NOT NULL
    DROP TABLE dbo.Loans;
GO

IF OBJECT_ID(N'dbo.Members', N'U') IS NOT NULL
    DROP TABLE dbo.Members;
GO

IF OBJECT_ID(N'dbo.Borrowers', N'U') IS NOT NULL
    DROP TABLE dbo.Borrowers;
GO

IF OBJECT_ID(N'dbo.Books', N'U') IS NOT NULL
    DROP TABLE dbo.Books;
GO

IF OBJECT_ID(N'dbo.Authors', N'U') IS NOT NULL
    DROP TABLE dbo.Authors;
GO

CREATE TABLE dbo.Authors
(
    Id   NVARCHAR(50)  NOT NULL CONSTRAINT PK_Authors PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Bio  NVARCHAR(MAX) NULL
);
GO

CREATE TABLE dbo.Books
(
    Id            NVARCHAR(50)  NOT NULL CONSTRAINT PK_Books PRIMARY KEY,
    Title         NVARCHAR(300) NOT NULL,
    AuthorId      NVARCHAR(50)  NOT NULL,
    Isbn          NVARCHAR(20)  NOT NULL,
    PublishedYear INT           NOT NULL,
    CONSTRAINT FK_Books_Authors FOREIGN KEY (AuthorId) REFERENCES dbo.Authors (Id)
);
GO

CREATE INDEX IX_Books_AuthorId ON dbo.Books (AuthorId);
GO

CREATE TABLE dbo.Members
(
    Id    NVARCHAR(50)  NOT NULL CONSTRAINT PK_Members PRIMARY KEY,
    Name  NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NULL
);
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
GO

CREATE INDEX IX_Loans_MemberId ON dbo.Loans (MemberId);
GO
