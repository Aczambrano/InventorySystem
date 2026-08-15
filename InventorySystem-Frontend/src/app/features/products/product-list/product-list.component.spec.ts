import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductListComponent } from './product-list.component';
import { ProductService, Product } from '../../../core/services/product.service';
import { ToastService } from '../../../core/services/toast.service';
import { of } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  let mockProductService: jasmine.SpyObj<ProductService>;
  let mockToastService: jasmine.SpyObj<ToastService>;

  beforeEach(async () => {
    mockProductService = jasmine.createSpyObj('ProductService', ['getAll', 'delete']);
    mockToastService = jasmine.createSpyObj('ToastService', ['success', 'error']);

    // Setup default mock return values
    mockProductService.getAll.and.returnValue(of([
      { id: '1', name: 'Product 1', sku: 'SKU1', description: 'Desc 1', stocks: [] },
      { id: '2', name: 'Product 2', sku: 'SKU2', description: 'Desc 2', stocks: [] }
    ]));

    await TestBed.configureTestingModule({
      imports: [ProductListComponent],
      providers: [
        { provide: ProductService, useValue: mockProductService },
        { provide: ToastService, useValue: mockToastService },
        { provide: ActivatedRoute, useValue: { params: of({}) } }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load products on init', () => {
    expect(mockProductService.getAll).toHaveBeenCalled();
    expect(component.products.length).toBe(2);
    expect(component.products[0].name).toBe('Product 1');
  });

  it('should call delete on ProductService when deleteProduct is confirmed', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    mockProductService.delete.and.returnValue(of(undefined));

    component.deleteProduct('1');

    expect(mockProductService.delete).toHaveBeenCalledWith('1');
    expect(mockToastService.success).toHaveBeenCalledWith('Product deleted successfully');
    // It should reload products
    expect(mockProductService.getAll).toHaveBeenCalledTimes(2); // Once in init, once after delete
  });

  it('should not call delete on ProductService when deleteProduct is cancelled', () => {
    spyOn(window, 'confirm').and.returnValue(false);

    component.deleteProduct('1');

    expect(mockProductService.delete).not.toHaveBeenCalled();
  });

  it('should toggle stock view when openStocks is called', () => {
    const product = { id: '1', name: 'P1', sku: '1', description: 'd', stocks: [] };
    
    component.openStocks(product);
    expect(component.selectedProductId).toBe('1');

    component.openStocks(product); // Click again should close
    expect(component.selectedProductId).toBeNull();
  });
});
