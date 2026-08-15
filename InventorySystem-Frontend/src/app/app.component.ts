import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { ToastService } from './core/services/toast.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="toast-container">
      <div *ngFor="let toast of toastService.messages" class="toast" [ngClass]="'toast-' + toast.type">
        {{ toast.message }}
      </div>
    </div>
    <router-outlet></router-outlet>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      top: 1rem;
      right: 1rem;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }
    .toast {
      padding: 1rem 1.5rem;
      border-radius: 0.5rem;
      color: white;
      font-weight: 500;
      box-shadow: var(--shadow-lg);
      animation: slideIn 0.3s ease-out forwards;
    }
    .toast-success { background-color: var(--success); }
    .toast-error { background-color: var(--error); }
    .toast-warning { background-color: var(--warning); }
    
    @keyframes slideIn {
      from { transform: translateX(100%); opacity: 0; }
      to { transform: translateX(0); opacity: 1; }
    }
  `]
})
export class AppComponent {
  toastService = inject(ToastService);
}
