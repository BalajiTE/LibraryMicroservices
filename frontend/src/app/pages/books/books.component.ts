import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Author, Book } from '../../core/models/library.models';
import { AuthorsApi, BooksApi } from '../../core/services/library-api.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-books',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './books.component.html',
  styleUrl: './books.component.css'
})
export class BooksComponent implements OnInit {
  books = signal<Book[]>([]);
  authors = signal<Author[]>([]);
  title = '';
  authorId = '';
  isbn = '';
  publishedYear = new Date().getFullYear();
  error = signal('');

  constructor(
    private readonly booksApi: BooksApi,
    private readonly authorsApi: AuthorsApi,
    readonly auth: AuthService
  ) {}

  ngOnInit(): void {
    this.load();
    this.authorsApi.getAll().subscribe({
      next: (authors) => {
        this.authors.set(authors);
        if (!this.authorId && authors.length > 0) {
          this.authorId = authors[0].id;
        }
      }
    });
  }

  load(): void {
    this.booksApi.getAll().subscribe({
      next: (books) => this.books.set(books),
      error: () => this.error.set('Failed to load books.')
    });
  }

  authorName(authorId: string): string {
    return this.authors().find((author) => author.id === authorId)?.name ?? authorId;
  }

  create(): void {
    this.booksApi
      .create({
        title: this.title,
        authorId: this.authorId,
        isbn: this.isbn,
        publishedYear: this.publishedYear
      })
      .subscribe({
        next: () => {
          this.title = '';
          this.isbn = '';
          this.load();
        },
        error: (err) => this.error.set(err?.error?.error ?? 'Create failed.')
      });
  }

  remove(book: Book): void {
    this.booksApi.delete(book.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.error ?? 'Delete failed.')
    });
  }
}
