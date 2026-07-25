import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../shared/models/product';

const baseURL = "http://localhost:5130/api/Products";

@Service()
export class ShopService {
    private httpClient = inject(HttpClient)
    types: string[] = [];
    brands: string[] = [];

    getAllProudcts(): Observable<Product[]> {
        return this.httpClient.get<Product[]>(`${baseURL}/GetProducts`);
    }

    getBrands() {
        if (this.brands.length > 0) return;
        this.httpClient.get<string[]>(`${baseURL}/GetBrands/brands`).subscribe({
            next: (data)=> this.brands = data
        })
    }

    getTypes() {
        if (this.types.length > 0) return;
        this.httpClient.get<string[]>(`${baseURL}/GetTypes/types`).subscribe({
            next: (data)=> this.types = data
        })
    }

}
