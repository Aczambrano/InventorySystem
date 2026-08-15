import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Provider {
  id: string;
  name: string;
  taxId: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ProviderService {
  private apiUrl = 'https://localhost:7041/api/Provider';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Provider[]> {
    return this.http.get<Provider[]>(this.apiUrl);
  }
}
