import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Author } from '../../core/models/library.models';
import { AuthorsApi } from '../../core/services/library-api.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-authors',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './authors.component.html',
  styleUrl: './authors.component.css'
})
export class AuthorsComponent implements OnInit {
  authors = signal<Author[]>([]);
  name = '';
  bio = '';
  error = signal('');

  constructor(
    private readonly api: AuthorsApi,
    readonly auth: AuthService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getAll().subscribe({
      next: (authors) => this.authors.set(authors),
      error: () => this.error.set('Failed to load authors.')
    });
  }

  create(): void {
    this.api.create(this.name, this.bio || null).subscribe({
      next: () => {
        this.name = '';
        this.bio = '';
        this.load();
      },
      error: (err) => this.error.set(err?.error?.error ?? 'Create failed.')
    });
  }

  remove(author: Author): void {
    this.api.delete(author.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.error ?? 'Delete failed.')
    });
  }
}
