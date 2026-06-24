import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { TokenResponse, User } from '../models/library.models';

const TOKEN_KEY = 'library_token';
const USER_KEY = 'library_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly currentUser = signal<User | null>(this.loadUser());

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router
  ) {}

  get token(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  get isLoggedIn(): boolean {
    return !!this.token;
  }

  login(username: string, password: string): Observable<TokenResponse> {
    return this.http
      .post<TokenResponse>('/api/auth/login', { username, password })
      .pipe(tap((response) => this.persistSession(response)));
  }

  register(username: string, email: string, password: string): Observable<TokenResponse> {
    return this.http
      .post<TokenResponse>('/api/auth/register', { username, email, password })
      .pipe(tap((response) => this.persistSession(response)));
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUser.set(null);
    void this.router.navigate(['/login']);
  }

  hasRole(...roles: string[]): boolean {
    const user = this.currentUser();
    if (!user) {
      return false;
    }

    return roles.some((role) => user.roles.includes(role));
  }

  canManageCatalog(): boolean {
    return this.hasRole('Admin', 'Librarian');
  }

  private persistSession(response: TokenResponse): void {
    const user: User = {
      ...response.user,
      roles: response.user.roles ?? []
    };

    localStorage.setItem(TOKEN_KEY, response.accessToken);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this.currentUser.set(user);
  }

  private loadUser(): User | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as User;
    } catch {
      return null;
    }
  }
}
