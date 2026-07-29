import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { AccountService } from '../../../services/AccountService';
import { extractErrorMessage } from '../../../shared/utils/extract-error-message';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private accountService = inject(AccountService);
  private router = inject(Router);

  errorMessage = '';

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
  });

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.errorMessage = '';

    this.accountService.login(this.loginForm.value as { email: string, password: string }).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (err) => this.errorMessage = extractErrorMessage(err),
    });
  }
}
