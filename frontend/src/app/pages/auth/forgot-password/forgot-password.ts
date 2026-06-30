import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SessionService } from '../../../services/session.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss',
})
export class ForgotPasswordComponent {
  forgotForm: FormGroup;
  message: string = '';
  isError: boolean = false;
  isLoading: boolean = false; 
  
  constructor(private fb: FormBuilder, private sessionService: SessionService) {
    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit() {
    if (this.forgotForm.invalid) return;

    this.isLoading = true;
    const email = this.forgotForm.value.email;

    this.sessionService.forgotPassword({ email }).subscribe({
      next: (response) => {
        this.isError = false;
        this.message = 'If your email is registered, you will receive a reset link shortly.';
        this.isLoading = false;
        this.forgotForm.reset();
      },
      error: (err) => {
        this.isError = true;
        this.message = 'An error occurred. Please try again later.';
        this.isLoading = false;
      }
    });
  }
}
