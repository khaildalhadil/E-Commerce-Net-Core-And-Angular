import { Component, inject, Input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../services/cartService';
import { Price } from '../../../shared/price/price';

/** Below this we name the exact number left instead of a generic "in stock". */
const LOW_STOCK_THRESHOLD = 5;

@Component({
  selector: 'app-product-item',
  imports: [RouterLink, Price],
  templateUrl: './product-item.html',
  styleUrl: './product-item.css',
})
export class ProductItem {
  @Input() product?: Product;

  private cartService = inject(CartService);

  get stockCount(): number {
    return this.product?.quantitiyInStock ?? 0;
  }

  get inStock(): boolean {
    return this.stockCount > 0;
  }

  get isLowStock(): boolean {
    return this.stockCount > 0 && this.stockCount <= LOW_STOCK_THRESHOLD;
  }

  get stockLabel(): string {
    if (!this.inStock) return 'Out of stock';
    if (this.isLowStock) return `Only ${this.stockCount} left`;
    return 'In stock';
  }

  addToCart() {
    if (this.product && this.inStock) this.cartService.addItemToCart(this.product);
  }
}
