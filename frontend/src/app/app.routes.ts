import { Routes } from '@angular/router';
import { authGuard, librarianGuard } from './core/guards/auth.guard';
import { ShellComponent } from './layout/shell/shell.component';
import { LoginComponent } from './pages/login/login.component';
import { AuthorsComponent } from './pages/authors/authors.component';
import { BooksComponent } from './pages/books/books.component';
import { MembersComponent } from './pages/members/members.component';
import { LoansComponent } from './pages/loans/loans.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: '',
    component: ShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'books' },
      { path: 'books', component: BooksComponent },
      { path: 'authors', component: AuthorsComponent },
      { path: 'members', component: MembersComponent, canActivate: [librarianGuard] },
      { path: 'loans', component: LoansComponent, canActivate: [authGuard] }
    ]
  },
  { path: '**', redirectTo: 'books' }
];
