import { Component, inject, Input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { DecimalPipe } from '@angular/common';
import { MatIcon } from "@angular/material/icon";
import { RouterLink } from '@angular/router';
import { CartService } from '../../../services/cartService';

@Component({
  selector: 'app-product-item',
  imports: [MatCardModule, MatButtonModule, DecimalPipe, MatIcon, RouterLink],
  templateUrl: './product-item.html',
  styleUrl: './product-item.css',
})
export class ProductItem {
  @Input() product?: Product;

  private cartService = inject(CartService);

  addToCart() {
    if (this.product) this.cartService.addItemToCart(this.product);
  }
}
