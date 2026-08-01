import { HttpClient } from '@angular/common/http';
import { computed, inject, Service, signal } from '@angular/core';
import { nanoid } from 'nanoid';
import { Observable, tap } from 'rxjs';
import { Product } from '../shared/models/product';
import { CartItem, ShoppingCart } from '../shared/models/cart';

const baseURL = "http://localhost:5130/api/Cart";
const STORAGE_KEY = "cart_id";

@Service()
export class CartService {
    private httpClient = inject(HttpClient);

    cart = signal<ShoppingCart | null>(null);

    itemCount = computed(() =>
        this.cart()?.items.reduce((sum, item) => sum + item.quantity, 0) ?? 0
    );

    subtotal = computed(() =>
        this.cart()?.items.reduce((sum, item) => sum + item.price * item.quantity, 0) ?? 0
    );

    constructor() {
        const cartId = localStorage.getItem(STORAGE_KEY);
        if (cartId) this.getCart(cartId).subscribe();
    }

    getCart(id: string): Observable<ShoppingCart> {
        return this.httpClient.get<ShoppingCart>(baseURL, { params: { id } }).pipe(
            tap(cart => this.cart.set(cart))
        );
    }

    setCart(cart: ShoppingCart): Observable<ShoppingCart> {
        return this.httpClient.post<ShoppingCart>(baseURL, cart).pipe(
            tap(updatedCart => {
                localStorage.setItem(STORAGE_KEY, updatedCart.id);
                this.cart.set(updatedCart);
            })
        );
    }

    deleteCart() {
        const cart = this.cart();
        if (!cart) return;

        this.httpClient.delete(baseURL, { params: { id: cart.id } }).subscribe({
            next: () => {
                localStorage.removeItem(STORAGE_KEY);
                this.cart.set(null);
            }
        });
    }

    addItemToCart(product: Product, quantity = 1) {
        const cart = this.cart() ?? this.createCart();
        const existingItem = cart.items.find(item => item.productId === product.id);

        if (existingItem) {
            existingItem.quantity += quantity;
        } else {
            cart.items.push(this.mapProductToCartItem(product, quantity));
        }

        this.cart.set({ ...cart });
        this.setCart(cart).subscribe();
    }

    incrementItemQuantity(productId: number) {
        const cart = this.cart();
        const item = cart?.items.find(item => item.productId === productId);
        if (!cart || !item) return;

        item.quantity++;
        this.cart.set({ ...cart });
        this.setCart(cart).subscribe();
    }

    decrementItemQuantity(productId: number) {
        const cart = this.cart();
        const item = cart?.items.find(item => item.productId === productId);
        if (!cart || !item) return;

        item.quantity--;
        if (item.quantity <= 0) {
            this.removeItemFromCart(productId);
        } else {
            this.cart.set({ ...cart });
            this.setCart(cart).subscribe();
        }
    }

    removeItemFromCart(productId: number) {
        const cart = this.cart();
        if (!cart) return;

        cart.items = cart.items.filter(item => item.productId !== productId);

        if (cart.items.length === 0) {
            this.deleteCart();
        } else {
            this.cart.set({ ...cart });
            this.setCart(cart).subscribe();
        }
    }

    private createCart(): ShoppingCart {
        const cart: ShoppingCart = { id: nanoid(), items: [] };
        localStorage.setItem(STORAGE_KEY, cart.id);
        return cart;
    }

    private mapProductToCartItem(product: Product, quantity: number): CartItem {
        return {
            productId: product.id,
            productName: product.name,
            price: product.price,
            quantity,
            pictureUrl: product.pictureUrl,
            brand: product.brand,
            type: product.type,
        };
    }
}
