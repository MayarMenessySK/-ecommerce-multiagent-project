export interface Cart {
  id: string;
  userId: string | null;
  sessionId: string | null;
  items: CartItem[];
  subtotal: number;
  tax: number;
  shipping: number;
  discount: number;
  total: number;
  discountCode: string | null;
}

export interface CartItem {
  id: string;
  cartId: string;
  productId: string;
  productName: string;
  productImage: string | null;
  price: number;
  quantity: number;
  subtotal: number;
  stockQuantity: number;
}

export interface AddToCartRequest {
  productId: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  cartItemId: string;
  quantity: number;
}

export interface ApplyDiscountRequest {
  discountCode: string;
}
