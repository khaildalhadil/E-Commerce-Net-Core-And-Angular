import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductType } from '../types/product-type';

const baseURL = "http://localhost:5130/api/Products";

@Service()
export class ProductServices {
    private httpClient = inject(HttpClient)

    GetAllProudcts(): Observable<ProductType[]> {
        return this.httpClient.get<ProductType[]>(`${baseURL}/GetProducts`);
    }

}
