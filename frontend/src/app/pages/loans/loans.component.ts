import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Book, Loan, Member } from '../../core/models/library.models';
import { BooksApi, LoansApi, MembersApi } from '../../core/services/library-api.service';
import { AuthService } from '../../core/services/auth.service';
import { formatHttpError } from '../../core/utils/http-error';

@Component({
  selector: 'app-loans',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './loans.component.html',
  styleUrl: './loans.component.css'
})
export class LoansComponent implements OnInit {
  loans = signal<Loan[]>([]);
  books = signal<Book[]>([]);
  members = signal<Member[]>([]);
  bookId = '';
  memberId = '';
  loanDate = new Date().toISOString().slice(0, 10);
  error = signal('');

  constructor(
    private readonly loansApi: LoansApi,
    private readonly booksApi: BooksApi,
    private readonly membersApi: MembersApi,
    readonly auth: AuthService
  ) {}

  ngOnInit(): void {
    this.load();

    this.booksApi.getAll().subscribe({
      next: (books) => {
        this.books.set(books);
        if (books[0]) {
          this.bookId = books[0].id;
        }
      }
    });

    if (this.auth.canManageCatalog()) {
      this.membersApi.getAll().subscribe({
        next: (members) => {
          this.members.set(members);
          if (members[0]) {
            this.memberId = members[0].id;
          }
        },
        error: (err) => this.error.set(formatHttpError(err, 'Failed to load members.'))
      });
    }
  }

  load(): void {
    this.loansApi.getAll().subscribe({
      next: (loans) => this.loans.set(loans),
      error: (err) => this.error.set(formatHttpError(err, 'Failed to load loans.'))
    });
  }

  bookTitle(bookId: string): string {
    return this.books().find((book) => book.id === bookId)?.title ?? bookId;
  }

  create(): void {
    this.loansApi.create(this.bookId, this.memberId, this.loanDate).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.error ?? 'Create failed.')
    });
  }

  returnLoan(loan: Loan): void {
    const returnDate = new Date().toISOString().slice(0, 10);
    this.loansApi.returnLoan(loan.id, returnDate).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.error ?? 'Return failed.')
    });
  }

  remove(loan: Loan): void {
    this.loansApi.delete(loan.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.error ?? 'Delete failed.')
    });
  }
}
