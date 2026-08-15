import { Component, EventEmitter, Input, Output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductStock, ProductService } from '../../../core/services/product.service';
import { ProviderService, Provider } from '../../../core/services/provider.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-product-stock',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="p-4">
      <div class="flex justify-between items-center mb-4">
        <h4>Stock Details</h4>
        <button (click)="closeView.emit()" class="btn btn-secondary btn-sm">Close</button>
      </div>

      <!-- Add Stock Form -->
      <form [formGroup]="stockForm" (ngSubmit)="onSubmit()" class="flex gap-4 items-end mb-6">
        <div class="form-group flex-1">
          <label>Provider</label>
          <select formControlName="providerId">
            <option value="">Select Provider...</option>
            <option *ngFor="let p of providers" [value]="p.id">{{ p.name }}</option>
          </select>
        </div>
        <div class="form-group flex-1">
          <label>Lot Number</label>
          <input type="text" formControlName="lotNumber" placeholder="Lot #" />
        </div>
        <div class="form-group flex-1">
          <label>Unit Price</label>
          <input type="number" formControlName="unitPrice" placeholder="0.00" step="0.01" />
        </div>
        <div class="form-group flex-1">
          <label>Quantity</label>
          <input type="number" formControlName="stockQuantity" placeholder="0" />
        </div>
        <button type="submit" class="btn btn-primary" [disabled]="stockForm.invalid || isLoading">
          {{ isEditing ? 'Update' : 'Add Stock' }}
        </button>
        <button *ngIf="isEditing" type="button" (click)="cancelEdit()" class="btn btn-secondary">Cancel</button>
      </form>

      <!-- Stocks Table -->
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>Provider</th>
              <th>Lot Number</th>
              <th>Price</th>
              <th>Quantity</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let stock of stocks">
              <td>{{ stock.providerName || 'Unknown' }}</td>
              <td>{{ stock.lotNumber }}</td>
              <td>\${{ stock.unitPrice | number:'1.2-2' }}</td>
              <td>{{ stock.stockQuantity }}</td>
              <td>
                <div class="flex gap-4">
                  <button (click)="editStock(stock)" class="btn btn-secondary btn-sm">Edit</button>
                  <button (click)="deleteStock(stock.id)" class="btn btn-danger btn-sm">Delete</button>
                </div>
              </td>
            </tr>
            <tr *ngIf="!stocks || stocks.length === 0">
              <td colspan="5" class="text-center text-secondary">No stock entries found.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .p-4 { padding: 1rem; }
    .mb-4 { margin-bottom: 1rem; }
    .mb-6 { margin-bottom: 1.5rem; }
    .flex-1 { flex: 1; }
    .btn-sm { padding: 0.25rem 0.75rem; font-size: 0.875rem; }
  `]
})
export class ProductStockComponent implements OnInit {
  @Input() productId!: string;
  @Input() stocks: ProductStock[] = [];
  @Output() stockUpdated = new EventEmitter<void>();
  @Output() closeView = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private providerService = inject(ProviderService);
  private toastService = inject(ToastService);

  providers: Provider[] = [];
  isLoading = false;
  isEditing = false;
  editingStockId: string | null = null;

  stockForm: FormGroup = this.fb.group({
    providerId: ['', Validators.required],
    lotNumber: ['', Validators.required],
    unitPrice: [0, [Validators.required, Validators.min(0)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit() {
    this.providerService.getAll().subscribe({
      next: (data) => this.providers = data,
      error: () => this.toastService.error('Failed to load providers')
    });
  }

  onSubmit() {
    if (this.stockForm.invalid) return;

    this.isLoading = true;
    const formValue = this.stockForm.value;

    if (this.isEditing && this.editingStockId) {
      // API currently takes same shape for update, but ignores ProviderId change if backend isn't mapped for it. 
      // Based on API: update takes LotNumber, UnitPrice, StockQuantity, ExpirationDate
      this.productService.updateStock(this.editingStockId, {
        lotNumber: formValue.lotNumber,
        unitPrice: formValue.unitPrice,
        stockQuantity: formValue.stockQuantity,
        expirationDate: null
      }).subscribe({
        next: () => {
          this.toastService.success('Stock updated');
          this.finishOperation();
        },
        error: () => {
          this.toastService.error('Failed to update stock');
          this.isLoading = false;
        }
      });
    } else {
      this.productService.addStock(this.productId, {
        providerId: formValue.providerId,
        lotNumber: formValue.lotNumber,
        unitPrice: formValue.unitPrice,
        stockQuantity: formValue.stockQuantity,
        expirationDate: null
      }).subscribe({
        next: () => {
          this.toastService.success('Stock added');
          this.finishOperation();
        },
        error: () => {
          this.toastService.error('Failed to add stock');
          this.isLoading = false;
        }
      });
    }
  }

  editStock(stock: ProductStock) {
    this.isEditing = true;
    this.editingStockId = stock.id;
    const matchingProvider = this.providers.find(p => p.name === stock.providerName);
    this.stockForm.patchValue({
      providerId: matchingProvider ? matchingProvider.id : '',
      lotNumber: stock.lotNumber,
      unitPrice: stock.unitPrice,
      stockQuantity: stock.stockQuantity
    });
    this.stockForm.get('providerId')?.disable(); // Prevent changing provider on edit
  }

  cancelEdit() {
    this.isEditing = false;
    this.editingStockId = null;
    this.stockForm.reset({ providerId: '', unitPrice: 0, stockQuantity: 0 });
    this.stockForm.get('providerId')?.enable();
  }

  deleteStock(stockId: string) {
    if (confirm('Delete this stock entry?')) {
      this.productService.deleteStock(stockId).subscribe({
        next: () => {
          this.toastService.success('Stock deleted');
          this.stockUpdated.emit();
        },
        error: () => this.toastService.error('Failed to delete stock')
      });
    }
  }

  private finishOperation() {
    this.isLoading = false;
    this.cancelEdit();
    this.stockUpdated.emit();
  }
}
