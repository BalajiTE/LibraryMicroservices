import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Author, Book, Loan, Member } from '../models/library.models';

@Injectable({ providedIn: 'root' })
export class AuthorsApi {
  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Author[]> {
    return this.http.get<Author[]>('/api/authors');
  }

  create(name: string, bio: string | null): Observable<Author> {
    return this.http.post<Author>('/api/authors', { name, bio });
  }

  update(id: string, name: string, bio: string | null): Observable<Author> {
    return this.http.put<Author>(`/api/authors/${id}`, { name, bio });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`/api/authors/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class BooksApi {
  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Book[]> {
    return this.http.get<Book[]>('/api/books');
  }

  create(book: Omit<Book, 'id'>): Observable<Book> {
    return this.http.post<Book>('/api/books', book);
  }

  update(id: string, book: Omit<Book, 'id'>): Observable<Book> {
    return this.http.put<Book>(`/api/books/${id}`, book);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`/api/books/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class MembersApi {
  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Member[]> {
    return this.http.get<Member[]>('/api/members');
  }

  create(name: string, email: string | null): Observable<Member> {
    return this.http.post<Member>('/api/members', { name, email });
  }

  update(id: string, name: string, email: string | null): Observable<Member> {
    return this.http.put<Member>(`/api/members/${id}`, { name, email });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`/api/members/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class LoansApi {
  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Loan[]> {
    return this.http.get<Loan[]>('/api/loans');
  }

  create(bookId: string, memberId: string, loanDate: string): Observable<Loan> {
    return this.http.post<Loan>('/api/loans', { bookId, memberId, loanDate });
  }

  returnLoan(id: string, returnDate: string): Observable<Loan> {
    return this.http.post<Loan>(`/api/loans/${id}/return`, { returnDate });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`/api/loans/${id}`);
  }
}
