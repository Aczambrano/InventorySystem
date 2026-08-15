import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="login-page flex items-center justify-center">
      <div class="glass-card login-card">
        <div class="login-header flex-col items-center mb-4">
          <div class="logo-icon">🔒</div>
          <h2>Welcome to Inventory<span class="text-accent">Pro</span></h2>
          <p class="text-secondary">Please sign in to continue</p>
        </div>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="flex-col gap-4 mt-8">
          <div class="form-group">
            <label for="email">Email</label>
            <input id="email" type="email" formControlName="email" placeholder="admin@example.com" />
            <div *ngIf="loginForm.get('email')?.invalid && loginForm.get('email')?.touched" class="text-error">
              Valid email is required
            </div>
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input id="password" type="password" formControlName="password" placeholder="••••••••" />
            <div *ngIf="loginForm.get('password')?.invalid && loginForm.get('password')?.touched" class="text-error">
              Password is required
            </div>
          </div>

          <button type="submit" class="btn btn-primary w-full mt-4" [disabled]="loginForm.invalid || isLoading">
            {{ isLoading ? 'Signing in...' : 'Sign In' }}
          </button>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .login-page {
      min-height: 100vh;
      background: radial-gradient(circle at top right, var(--bg-tertiary), var(--bg-primary));
    }
    .login-card {
      width: 100%;
      max-width: 400px;
    }
    .text-accent { color: var(--accent-primary); }
    .text-secondary { color: var(--text-secondary); }
    .logo-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
      background: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
    }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm: FormGroup = this.fb.group({
    email: ['admin@bancoguayaquil.com', [Validators.required, Validators.email]],
    password: ['Admin123!', Validators.required]
  });

  isLoading = false;

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      const { email, password } = this.loginForm.value;
      
      this.authService.login(email, password).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/dashboard']);
        },
        error: () => {
          this.isLoading = false;
        }
      });
    }
  }
}
