USE [LibraryMicroservices];
GO

IF OBJECT_ID(N'dbo.Loans', N'U') IS NOT NULL
    DROP TABLE dbo.Loans;
GO

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NOT NULL
    DROP TABLE dbo.UserRoles;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
    DROP TABLE dbo.Users;
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL
    DROP TABLE dbo.Roles;
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

CREATE TABLE dbo.Roles
(
    Id          NVARCHAR(50)  NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);
GO

CREATE TABLE dbo.Users
(
    Id           NVARCHAR(50)  NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Username     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    IsActive     BIT           NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

CREATE TABLE dbo.UserRoles
(
    UserId NVARCHAR(50) NOT NULL,
    RoleId NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles (Id) ON DELETE CASCADE
);
GO
