import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService, Product } from '../../../core/services/product.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard-page flex-col gap-6">
      <div class="header-section flex justify-between items-center">
        <h2>Dashboard Overview</h2>
      </div>
      
      <div class="stats-grid">
        <div class="stat-card glass-card">
          <div class="stat-title">Total Products</div>
          <div class="stat-value">{{ products.length }}</div>
        </div>
        <div class="stat-card glass-card">
          <div class="stat-title">Total Stock Value</div>
          <div class="stat-value">\${{ getTotalStockValue() | number:'1.2-2' }}</div>
        </div>
        <div class="stat-card glass-card">
          <div class="stat-title">Low Stock Alerts</div>
          <div class="stat-value text-error">{{ getLowStockCount() }}</div>
        </div>
      </div>

      <div class="glass-card mt-4">
        <h3>Recent Activity</h3>
        <p class="text-secondary mt-2">Welcome to the inventory system dashboard. Navigate to Products to start managing your catalog.</p>
      </div>
    </div>
  `,
  styles: [`
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
    }
    .stat-card {
      padding: 1.5rem;
    }
    .stat-title {
      color: var(--text-secondary);
      font-weight: 500;
      margin-bottom: 0.5rem;
    }
    .stat-value {
      font-size: 2rem;
      font-weight: 700;
      color: var(--text-primary);
    }
  `]
})
export class DashboardComponent implements OnInit {
  private productService = inject(ProductService);
  products: Product[] = [];

  ngOnInit() {
    this.productService.getAll().subscribe({
      next: (data) => this.products = data,
      error: () => {}
    });
  }

  getTotalStockValue(): number {
    return this.products.reduce((acc, product) => {
      const productTotal = product.stocks?.reduce((sum, stock) => sum + (stock.unitPrice * stock.stockQuantity), 0) || 0;
      return acc + productTotal;
    }, 0);
  }

  getLowStockCount(): number {
    return this.products.filter(p => p.stocks?.some(s => s.stockQuantity < 10)).length;
  }
}
