import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { SessionService } from '../../../services/session.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private sessionService = inject(SessionService);
  private router = inject(Router);

  roles = signal<any[]>([]);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  registerForm = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    roleId: [null as number | null, Validators.required]
  });

  constructor() {
    this.sessionService.getRoles().subscribe({
      next: (response) => {
        const rolesArray = response?.body || response?.data || response || [];
        this.roles.set(rolesArray);
      },
      error: (err) => console.error('Failed to load roles', err)
    });
  }

  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const formValue = this.registerForm.value;
    
    this.sessionService.register({
      name: formValue.name!,
      email: formValue.email!,
      password: formValue.password!,
      roleId: Number(formValue.roleId)
    }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.successMessage.set('Registration successful! You can now log in.');
        this.registerForm.reset();
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || 'Registration failed. Please try again.');
        console.error('Registration error', err);
      }
    });
  }
  navigateToLogin() {
    this.router.navigate(['/login']);
  }
}

