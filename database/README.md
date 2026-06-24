# LibraryMicroservices SQL Server Database

This folder contains scripts to create a local SQL Server database that mirrors the seed data in `data/*.json`.

## Database

| Setting | Value |
|---------|-------|
| **Name** | `LibraryMicroservices` |
| **Server** | `localhost` (Windows auth) |

## Tables

| Table | Source JSON | Description |
|-------|-------------|-------------|
| `Authors` | `data/authors.json` | Library authors |
| `Books` | `data/books.json` | Books linked to authors |
| `Members` | `data/members.json` | Library members who borrow books |
| `Loans` | `data/loans.json` | Book loans linked to members |
| `Roles` | `data/roles.json` | Auth roles (Admin, Librarian, Member) |
| `Users` | `data/users.json` | Login accounts |
| `UserRoles` | `data/userRoles.json` | User-to-role assignments |

## Quick setup

From PowerShell:

```powershell
cd C:\Me\Projects\LibraryMicroservices\database
.\setup-database.ps1
```

Optional parameters:

```powershell
.\setup-database.ps1 -Server "localhost"
.\setup-database.ps1 -Server "(localdb)\MSSQLLocalDB"
```

**Existing database missing auth tables** (adds `Users`, `Roles`, `UserRoles` without wiping catalog data):

```powershell
.\setup-database.ps1 -AuthOnly
```

## Manual setup

```powershell
sqlcmd -S localhost -E -i sql\01-create-database.sql
sqlcmd -S localhost -E -d LibraryMicroservices -i sql\02-create-tables.sql
sqlcmd -S localhost -E -d LibraryMicroservices -i sql\03-seed-data.sql
sqlcmd -S localhost -E -d LibraryMicroservices -i sql\04-add-auth-tables.sql
```

To upgrade an existing database that still has `Borrowers` / `BorrowerId`:

```powershell
sqlcmd -S localhost -E -d LibraryMicroservices -i sql\05-migrate-borrowers-to-members.sql
```

## Connection string

```
Server=localhost;Database=LibraryMicroservices;Trusted_Connection=True;TrustServerCertificate=True
```

The APIs use this via `ConnectionStrings:LibraryDatabase` when `Persistence:Provider` is `SqlServer`.

## Verify

```powershell
sqlcmd -S localhost -E -d LibraryMicroservices -Q "SELECT COUNT(*) AS AuthorCount FROM Authors; SELECT COUNT(*) AS BookCount FROM Books; SELECT COUNT(*) AS LoanCount FROM Loans;"
```

Expected counts: **3 authors**, **3 books**, **2 borrowers**, **2 loans**.
