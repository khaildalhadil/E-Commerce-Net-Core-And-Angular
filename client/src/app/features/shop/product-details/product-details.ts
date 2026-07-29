import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DecimalPipe } from '@angular/common';
import { Product } from '../../../shared/models/product';
import { ShopService } from '../../../services/ShopService';

@Component({
  selector: 'app-product-details',
  imports: [MatCardModule, MatButtonModule, MatIcon, MatProgressSpinnerModule, DecimalPipe, RouterLink],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css',
})
export class ProductDetails implements OnInit {

  private route = inject(ActivatedRoute);
  private shopService = inject(ShopService);

  product = signal<Product | undefined>(undefined);
  isLoading = signal<boolean>(true);
  notFound = signal<boolean>(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.shopService.getProduct(id).subscribe({
      next: (data) => this.product.set(data),
      error: (err) => {
        console.log(err);
        this.notFound.set(true);
      },
      complete: () => this.isLoading.set(false),
    });
  }

}
