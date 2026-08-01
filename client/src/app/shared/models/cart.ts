
export interface CartItem {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand: string;
  type: string;
}

export interface ShoppingCart {
  id: string;
  items: CartItem[];
}
