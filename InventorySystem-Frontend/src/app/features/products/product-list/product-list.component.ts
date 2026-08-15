import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService, Product } from '../../../core/services/product.service';
import { ToastService } from '../../../core/services/toast.service';
import { ProductStockComponent } from '../product-stock/product-stock.component';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink, ProductStockComponent],
  template: `
    <div class="product-list-page flex-col gap-6">
      <div class="header-section flex justify-between items-center">
        <h2>Products Management</h2>
        <a routerLink="/products/new" class="btn btn-primary">+ Add Product</a>
      </div>

      <div class="glass-card">
        <div class="table-container">
          <table>
            <thead>
              <tr>
                <th>SKU</th>
                <th>Name</th>
                <th>Description</th>
                <th>Total Stock</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <ng-container *ngFor="let product of products">
                <tr>
                  <td>{{ product.sku }}</td>
                  <td><strong>{{ product.name }}</strong></td>
                  <td>{{ product.description }}</td>
                  <td>
                    <span class="badge" [ngClass]="getTotalStock(product) > 10 ? 'badge-success' : 'badge-error'">
                      {{ getTotalStock(product) }}
                    </span>
                  </td>
                  <td>
                    <div class="flex gap-4">
                      <button (click)="openStocks(product)" class="btn btn-secondary btn-sm">Stocks</button>
                      <a [routerLink]="['/products/edit', product.id]" class="btn btn-secondary btn-sm">Edit</a>
                      <button (click)="deleteProduct(product.id)" class="btn btn-danger btn-sm">Delete</button>
                    </div>
                  </td>
                </tr>
                <!-- Inline Stock View -->
                <tr *ngIf="selectedProductId === product.id">
                  <td colspan="5" class="stock-details-row">
                    <app-product-stock [productId]="product.id" [stocks]="product.stocks" (stockUpdated)="loadProducts()" (closeView)="closeStocks()"></app-product-stock>
                  </td>
                </tr>
              </ng-container>
              <tr *ngIf="products.length === 0">
                <td colspan="5" class="text-center p-4 text-secondary">No products found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .btn-sm { padding: 0.25rem 0.75rem; font-size: 0.875rem; }
    .stock-details-row {
      background-color: rgba(0,0,0,0.2);
    }
  `]
})
export class ProductListComponent implements OnInit {
  private productService = inject(ProductService);
  private toastService = inject(ToastService);

  products: Product[] = [];
  selectedProductId: string | null = null;

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getAll().subscribe({
      next: (data) => this.products = data,
      error: () => this.toastService.error('Failed to load products')
    });
  }

  getTotalStock(product: Product): number {
    return product.stocks?.reduce((sum, stock) => sum + stock.stockQuantity, 0) || 0;
  }

  deleteProduct(id: string) {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.delete(id).subscribe({
        next: () => {
          this.toastService.success('Product deleted successfully');
          this.loadProducts();
        },
        error: () => this.toastService.error('Failed to delete product')
      });
    }
  }

  openStocks(product: Product) {
    this.selectedProductId = this.selectedProductId === product.id ? null : product.id;
  }

  closeStocks() {
    this.selectedProductId = null;
  }
}
