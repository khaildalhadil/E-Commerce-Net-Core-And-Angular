import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../shared/models/product';

const baseURL = "http://localhost:5130/api/Products";

@Service()
export class ProductServices {
    private httpClient = inject(HttpClient)

    GetAllProudcts(): Observable<Product[]> {
        return this.httpClient.get<Product[]>(`${baseURL}/GetProducts`);
    }

}
