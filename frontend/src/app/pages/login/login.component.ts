import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  username = 'librarian';
  password = 'Password123!';
  email = '';
  mode: 'login' | 'register' = 'login';
  error = signal('');

  constructor(
    private readonly auth: AuthService,
    private readonly router: Router
  ) {}

  submit(): void {
    this.error.set('');
    const request =
      this.mode === 'login'
        ? this.auth.login(this.username, this.password)
        : this.auth.register(this.username, this.email, this.password);

    request.subscribe({
      next: () => void this.router.navigate(['/books']),
      error: (err) => this.error.set(err?.error?.error ?? 'Authentication failed.')
    });
  }
}
