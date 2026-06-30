import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ContactComponent {
  readonly submitted = signal(false);

  model = {
    name: '',
    email: '',
    company: '',
    subject: '',
    message: ''
  };

  onSubmit(): void {
    this.submitted.set(true);
  }

  resetForm(): void {
    this.model = { name: '', email: '', company: '', subject: '', message: '' };
    this.submitted.set(false);
  }
}
