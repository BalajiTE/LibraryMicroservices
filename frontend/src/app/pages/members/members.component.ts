import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Member } from '../../core/models/library.models';
import { MembersApi } from '../../core/services/library-api.service';
import { formatHttpError } from '../../core/utils/http-error';

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './members.component.html',
  styleUrl: './members.component.css'
})
export class MembersComponent implements OnInit {
  members = signal<Member[]>([]);
  name = '';
  email = '';
  error = signal('');

  constructor(private readonly api: MembersApi) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getAll().subscribe({
      next: (members) => this.members.set(members),
      error: (err) => this.error.set(formatHttpError(err, 'Failed to load members.'))
    });
  }

  create(): void {
    this.api.create(this.name, this.email || null).subscribe({
      next: () => {
        this.name = '';
        this.email = '';
        this.load();
      },
      error: (err) => this.error.set(err?.error?.error ?? 'Create failed.')
    });
  }

  remove(member: Member): void {
    this.api.delete(member.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.error ?? 'Delete failed.')
    });
  }
}
