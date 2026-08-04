import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartService } from '../../services/cartService';
import { Price } from '../../shared/price/price';

@Component({
  selector: 'app-cart',
  imports: [RouterLink, Price],
  templateUrl: './cart.html',
  styleUrl: './cart.css',
})
export class Cart {
  cartService = inject(CartService);

  items = computed(() => this.cartService.cart()?.items ?? []);
}
