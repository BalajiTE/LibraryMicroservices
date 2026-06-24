USE [LibraryMicroservices];
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id          NVARCHAR(50)  NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        CONSTRAINT UQ_Roles_Name UNIQUE (Name)
    );
END
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
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
END
GO

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserRoles
    (
        UserId NVARCHAR(50) NOT NULL,
        RoleId NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (Id) ON DELETE CASCADE,
        CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles (Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Roles)
BEGIN
    INSERT INTO dbo.Roles (Id, Name, Description)
    VALUES
        (N'r1', N'Admin', N'Full system access including user management'),
        (N'r2', N'Librarian', N'Manage catalog, members, and loans'),
        (N'r3', N'Member', N'Browse catalog and view loans');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
    INSERT INTO dbo.Users (Id, Username, Email, PasswordHash, IsActive)
    VALUES
        (N'u1', N'admin', N'admin@library.local', N'AQAAAAIAAYagAAAAEHJUDbb3QN71tMQBNybl/mCkDvozC17kDjEgowMcj/8w+eMehO90wICy28A5m8wgxg==', 1),
        (N'u2', N'librarian', N'librarian@library.local', N'AQAAAAIAAYagAAAAEHJUDbb3QN71tMQBNybl/mCkDvozC17kDjEgowMcj/8w+eMehO90wICy28A5m8wgxg==', 1),
        (N'u3', N'member', N'member@library.local', N'AQAAAAIAAYagAAAAEHJUDbb3QN71tMQBNybl/mCkDvozC17kDjEgowMcj/8w+eMehO90wICy28A5m8wgxg==', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.UserRoles)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId)
    VALUES
        (N'u1', N'r1'),
        (N'u2', N'r2'),
        (N'u3', N'r3');
END
GO
