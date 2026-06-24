export interface Author {
  id: string;
  name: string;
  bio?: string | null;
}

export interface Book {
  id: string;
  title: string;
  authorId: string;
  isbn: string;
  publishedYear: number;
}

export interface Member {
  id: string;
  name: string;
  email?: string | null;
}

export interface Loan {
  id: string;
  bookId: string;
  memberId: string;
  memberName: string;
  loanDate: string;
  returnDate?: string | null;
}

export interface User {
  id: string;
  username: string;
  email: string;
  isActive: boolean;
  roles: string[];
}

export interface TokenResponse {
  accessToken: string;
  tokenType: string;
  expiresInMinutes: number;
  user: User;
}

export interface ApiError {
  error?: string;
  title?: string;
  detail?: string;
}
