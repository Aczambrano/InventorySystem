import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="layout-container flex">
      <!-- Sidebar -->
      <aside class="sidebar glass-card flex-col">
        <div class="sidebar-header">
          <h2>Inventory<span class="text-accent">Pro</span></h2>
        </div>
        <nav class="sidebar-nav flex-col gap-4 mt-8">
          <a routerLink="/dashboard" routerLinkActive="active" class="nav-link">
            <span class="icon">📊</span> Dashboard
          </a>
          <a routerLink="/products" routerLinkActive="active" class="nav-link">
            <span class="icon">📦</span> Products
          </a>
        </nav>
        <div class="sidebar-footer mt-auto">
          <button (click)="logout()" class="btn btn-secondary w-full">
            Logout
          </button>
        </div>
      </aside>

      <!-- Main Content -->
      <main class="main-content">
        <header class="topbar flex justify-between items-center glass-card">
          <h3>Welcome back!</h3>
          <div class="user-profile">
            <div class="avatar">Admin</div>
          </div>
        </header>
        <div class="content-area">
          <router-outlet></router-outlet>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .layout-container {
      min-height: 100vh;
      background-color: var(--bg-primary);
    }
    .sidebar {
      width: 260px;
      padding: 1.5rem;
      border-radius: 0;
      border-left: none;
      border-top: none;
      border-bottom: none;
      position: fixed;
      height: 100vh;
      display: flex;
    }
    .text-accent { color: var(--accent-primary); }
    .nav-link {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1rem;
      border-radius: 0.5rem;
      color: var(--text-secondary);
      font-weight: 500;
    }
    .nav-link:hover, .nav-link.active {
      background-color: rgba(59, 130, 246, 0.1);
      color: var(--accent-primary);
    }
    .main-content {
      flex: 1;
      margin-left: 260px;
      padding: 1.5rem;
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }
    .topbar {
      padding: 1rem 2rem;
      border-radius: 1rem;
    }
    .topbar h3 { margin: 0; font-size: 1.2rem; }
    .avatar {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 600;
      color: white;
      font-size: 0.75rem;
    }
  `]
})
export class LayoutComponent {
  private authService = inject(AuthService);

  logout() {
    this.authService.logout();
  }
}
