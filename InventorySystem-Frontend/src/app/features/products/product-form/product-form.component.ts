import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { ProviderService, Provider } from '../../../core/services/provider.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="product-form-page flex-col gap-6">
      <div class="header-section flex justify-between items-center">
        <h2>{{ isEditMode ? 'Edit Product' : 'Add New Product' }}</h2>
        <a routerLink="/products" class="btn btn-secondary">Back to List</a>
      </div>

      <div class="glass-card max-w-2xl">
        <form [formGroup]="productForm" (ngSubmit)="onSubmit()" class="flex-col gap-4">
          
          <div class="form-group">
            <label for="sku">SKU</label>
            <input id="sku" type="text" formControlName="sku" placeholder="PRD-001" />
            <div *ngIf="productForm.get('sku')?.invalid && productForm.get('sku')?.touched" class="text-error">
              SKU is required
            </div>
          </div>

          <div class="form-group">
            <label for="name">Product Name</label>
            <input id="name" type="text" formControlName="name" placeholder="E.g. 50 inch 4K Monitor" />
            <div *ngIf="productForm.get('name')?.invalid && productForm.get('name')?.touched" class="text-error">
              Name is required
            </div>
          </div>

          <div class="form-group">
            <label for="description">Description</label>
            <textarea id="description" formControlName="description" rows="4" placeholder="Product details..."></textarea>
          </div>

          <!-- Initial Stock Fields (Only when creating) -->
          <ng-container *ngIf="!isEditMode">
            <h4 class="mt-4 mb-2 text-accent">Initial Stock Details</h4>
            
            <div class="flex gap-4">
              <div class="form-group flex-1">
                <label>Provider</label>
                <select formControlName="providerId">
                  <option value="">Select Provider...</option>
                  <option *ngFor="let p of providers" [value]="p.id">{{ p.name }}</option>
                </select>
                <div *ngIf="productForm.get('providerId')?.invalid && productForm.get('providerId')?.touched" class="text-error">
                  Provider is required
                </div>
              </div>
              <div class="form-group flex-1">
                <label>Lot Number</label>
                <input type="text" formControlName="lotNumber" placeholder="Lot #" />
              </div>
            </div>

            <div class="flex gap-4">
              <div class="form-group flex-1">
                <label>Unit Price ($)</label>
                <input type="number" formControlName="unitPrice" placeholder="0.00" step="0.01" />
              </div>
              <div class="form-group flex-1">
                <label>Initial Quantity</label>
                <input type="number" formControlName="stockQuantity" placeholder="0" />
              </div>
            </div>
          </ng-container>

          <div class="mt-4 flex gap-4">
            <button type="submit" class="btn btn-primary" [disabled]="productForm.invalid || isLoading">
              {{ isLoading ? 'Saving...' : 'Save Product' }}
            </button>
            <a routerLink="/products" class="btn btn-secondary">Cancel</a>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .max-w-2xl { max-width: 42rem; }
    .flex-1 { flex: 1; }
    .text-accent { color: var(--accent-primary); font-size: 1.1rem; }
  `]
})
export class ProductFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private providerService = inject(ProviderService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastService = inject(ToastService);

  isEditMode = false;
  productId: string | null = null;
  isLoading = false;
  providers: Provider[] = [];

  productForm: FormGroup = this.fb.group({
    sku: ['', Validators.required],
    name: ['', Validators.required],
    description: [''],
    providerId: [''],
    lotNumber: [''],
    unitPrice: [0],
    stockQuantity: [0]
  });

  ngOnInit() {
    this.productId = this.route.snapshot.paramMap.get('id');
    if (this.productId) {
      this.isEditMode = true;
      this.loadProduct(this.productId);
    } else {
      // Set validators for creation mode
      this.productForm.get('providerId')?.setValidators(Validators.required);
      this.productForm.get('lotNumber')?.setValidators(Validators.required);
      this.productForm.get('unitPrice')?.setValidators([Validators.required, Validators.min(0.01)]);
      this.productForm.get('stockQuantity')?.setValidators([Validators.required, Validators.min(1)]);
      this.productForm.updateValueAndValidity();
      this.loadProviders();
    }
  }

  loadProviders() {
    this.providerService.getAll().subscribe({
      next: (data) => this.providers = data,
      error: () => this.toastService.error('Failed to load providers')
    });
  }

  loadProduct(id: string) {
    this.productService.getById(id).subscribe({
      next: (product) => {
        this.productForm.patchValue({
          sku: product.sku,
          name: product.name,
          description: product.description
        });
      },
      error: () => {
        this.toastService.error('Failed to load product details');
        this.router.navigate(['/products']);
      }
    });
  }

  onSubmit() {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }
    
    this.isLoading = true;
    const formValue = this.productForm.value;

    if (this.isEditMode && this.productId) {
      this.productService.update(this.productId, { 
        id: this.productId, 
        sku: formValue.sku, 
        name: formValue.name, 
        description: formValue.description 
      }).subscribe({
        next: () => {
          this.toastService.success('Product updated successfully');
          this.router.navigate(['/products']);
        },
        error: () => {
          this.isLoading = false;
        }
      });
    } else {
      this.productService.create({
        sku: formValue.sku,
        name: formValue.name,
        description: formValue.description,
        providerId: formValue.providerId,
        lotNumber: formValue.lotNumber,
        unitPrice: formValue.unitPrice,
        stockQuantity: formValue.stockQuantity,
        expirationDate: null
      }).subscribe({
        next: () => {
          this.toastService.success('Product created successfully');
          this.router.navigate(['/products']);
        },
        error: () => {
          this.isLoading = false;
        }
      });
    }
  }
}
