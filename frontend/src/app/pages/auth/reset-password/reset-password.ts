import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SessionService } from '../../../services/session.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss',
})
export class ResetPasswordComponent implements OnInit {
  resetForm: FormGroup;
  token: string = '';
  email: string = '';
  message: string = '';
  isError: boolean = false;
  isSuccess: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private sessionService: SessionService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    //create form group with password validation
    this.resetForm = this.fb.group({
      newPassword: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$/)]], //Regex(Regular Expression).
      confirmPassword: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    // Reading data from the link (Query Params)
    this.token = this.route.snapshot.queryParamMap.get('token') || '';
    this.email = this.route.snapshot.queryParamMap.get('email') || '';

    if (!this.token || !this.email) {
      this.isError = true;
      this.message = 'Reset password link is invalid or expired.';
    }
  }

  onSubmit() {
    if (this.resetForm.invalid) return;

    const { newPassword, confirmPassword } = this.resetForm.value;

    if (newPassword !== confirmPassword) {
      this.isError = true;
      this.message = 'Passwords do not match.';
      return;
    }

    const resetData = {
      token: this.token,
      email: this.email,
      newPassword: newPassword,
      confirmPassword: confirmPassword
    };

    // Call the service to reset the password
    this.sessionService.resetPassword(resetData).subscribe({
      next: (res: any) => {
        // Success: Use the message coming from the backend Ok()
        this.isError = false;
        this.isSuccess = true;
        this.message = res.message || 'Password updated successfully!';
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isError = true;
        this.isSuccess = false;
        // Extract error message from the backend BadRequest(new { message = "..." })
        this.message = err.error?.message || 'An unexpected error occurred. Please try again.';
        this.cdr.detectChanges();
      }
    });
  }
  navigateToLogin() {
    this.router.navigate(['/login']);
  }
}
