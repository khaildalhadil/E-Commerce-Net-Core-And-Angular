import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../shared/models/product';
import { CreateProduct } from '../shared/models/createProduct';

const baseURL = "http://localhost:5130/api/Products";

@Service()
export class ShopService {
    private httpClient = inject(HttpClient)
    types: string[] = [];
    brands: string[] = [];

    getAllProudcts(brands?: string[], types?: string[]): Observable<Product[]> {
        
        let params = new HttpParams();

        if(brands && brands.length > 0) params = params.append('brand', brands.join(','))
        if(types && types.length > 0) params = params.append('type', types.join(','))

        return this.httpClient.get<Product[]>(`${baseURL}/GetProducts`, {params});
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
    AddProduct(product: CreateProduct): Observable<Product> {
        return this.httpClient.post<Product>(`${baseURL}/CreatePoroduct`, product);
    }

    getProduct(id: number): Observable<Product> {
        return this.httpClient.get<Product>(`${baseURL}/GetProduct/${id}`);
    }

    GerImage(promat: string): Observable<string>{
        return this.httpClient.post<string>(`http://localhost:5130/api/Image/generate`, {prompt: promat});
    }

}
