import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProductStock {
  id: string;
  providerName: string;
  lotNumber: string;
  unitPrice: number;
  stockQuantity: number;
  expirationDate: string | null;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  sku: string;
  stocks: ProductStock[];
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'https://localhost:7041/api/Product';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  create(product: { name: string; description: string; sku: string; providerId: string; lotNumber: string; unitPrice: number; stockQuantity: number; expirationDate: string | null }): Observable<string> {
    return this.http.post<string>(this.apiUrl, product);
  }

  update(id: string, product: { id: string; name: string; description: string; sku: string }): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, product);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Stock operations
  addStock(productId: string, stock: { providerId: string; lotNumber: string; unitPrice: number; stockQuantity: number; expirationDate: string | null }): Observable<{ stockId: string }> {
    return this.http.post<{ stockId: string }>(`${this.apiUrl}/${productId}/stocks`, stock);
  }

  updateStock(stockId: string, stock: { lotNumber: string; unitPrice: number; stockQuantity: number; expirationDate: string | null }): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/stocks/${stockId}`, stock);
  }

  deleteStock(stockId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/stocks/${stockId}`);
  }
}
