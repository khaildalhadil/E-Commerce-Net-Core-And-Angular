import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { Product } from '../../shared/models/product';
import { ProductServices } from '../../services/ShopService';
import { ProductItem } from './product-item/product-item';

@Component({
  selector: 'app-shop',
  imports: [ProductItem],
  templateUrl: './shop.html',
  styleUrl: './shop.css',
})
export class Shop implements OnInit {

  

  private productServices = inject(ProductServices);
  public allProducts = signal<Product[]>([])
  
  ngOnInit(): void {
    this.GetAllProduct()
    this.initializeShop();
  }

  GetAllProduct() {

    this.productServices.getAllProudcts().subscribe({

      next: (data) => {
        this.allProducts.set(data);
        console.log(data);
      },

      error: (err) => {
        console.log(err);
      },

      complete: () => {
        console.log("Complate")
      }

    })
  }

  initializeShop() {
    this.productServices.getBrands();
    this.productServices.getTypes();
  }
}
