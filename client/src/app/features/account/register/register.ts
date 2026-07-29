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
  selector: 'app-register',
  imports: [ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private accountService = inject(AccountService);
  private router = inject(Router);

  errorMessage = '';

  registerForm = new FormGroup({
    displayName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(40),
    ]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.errorMessage = '';

    this.accountService.register(this.registerForm.value as {
      displayName: string, email: string, password: string
    }).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (err) => this.errorMessage = extractErrorMessage(err),
    });
  }
}
