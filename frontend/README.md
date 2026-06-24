# Library Portal (Angular 18)

Angular UI for **LibraryMicroservices**, talking to the API gateway at `http://localhost:5000`.

## Prerequisites

- Node.js 18+
- API gateway and backend services running

## Run

```powershell
# Terminal 1: backend services + gateway
cd C:\Me\Projects\LibraryMicroservices
.\scripts\start-all.ps1

# Terminal 2: Angular dev server
cd C:\Me\Projects\LibraryMicroservices\frontend
npm start
```

Open http://localhost:4200

The dev server proxies `/api/*` to the gateway via `proxy.conf.json`.

## Seed logins

| Username | Password | Role |
|----------|----------|------|
| `admin` | `Password123!` | Admin |
| `librarian` | `Password123!` | Librarian |
| `member` | `Password123!` | Member |

## Features

- Public catalog: browse **Books** and **Authors**
- JWT login / register
- Role-aware UI:
  - **Librarian/Admin**: manage authors, books, members, loans
  - **Member**: view loans
- Automatic `Authorization: Bearer` header via HTTP interceptor

## Build

```powershell
npm run build
```

Output: `dist/frontend`
