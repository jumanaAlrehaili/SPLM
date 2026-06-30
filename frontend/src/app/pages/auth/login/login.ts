import { Component, ChangeDetectorRef } from '@angular/core';
import { SessionService } from '../../../services/session.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {

  model: any = {};
  errorMessage: string = '';

  constructor(
    private sessionService: SessionService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) { }

  onSubmit() {
    this.errorMessage = '';

    this.sessionService.login(this.model).subscribe({
      next: (response: any) => {
        console.log('Login response:', response);

        this.router.navigate(['/dashboard/projects']);
      },
      error: err => {
        console.log('Login failed:', err);
        this.errorMessage = err.error || 'Something went wrong';
        this.cdr.detectChanges();
      }
    });
  }
}
