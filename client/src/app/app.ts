import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./shared/header/header";
import { ProductServices } from './services/product-services';
import { Product } from './shared/models/product';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App implements OnInit {
  
  private productServices = inject(ProductServices);
  public allProducts = signal<Product[]>([])
  
  ngOnInit(): void {
    this.GetAllProduct()
  }

  GetAllProduct() {

    this.productServices.GetAllProudcts().subscribe({

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

}
