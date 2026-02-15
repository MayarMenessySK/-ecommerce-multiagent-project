export interface Order {
  id: string;
  orderNumber: string;
  userId: string;
  status: OrderStatus;
  subtotal: number;
  tax: number;
  shippingCost: number;
  discount: number;
  total: number;
  paymentMethod: string;
  paymentStatus: PaymentStatus;
  shippingAddress: ShippingAddress;
  items: OrderItem[];
  trackingNumber: string | null;
  createdAt: string;
  deliveredAt: string | null;
}

export enum OrderStatus {
  Pending = 'Pending',
  Processing = 'Processing',
  Shipped = 'Shipped',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled'
}

export enum PaymentStatus {
  Pending = 'Pending',
  Completed = 'Completed',
  Failed = 'Failed',
  Refunded = 'Refunded'
}

export interface ShippingAddress {
  fullName: string;
  phoneNumber: string;
  addressLine1: string;
  addressLine2: string | null;
  city: string;
  state: string;
  postalCode: string;
  country: string;
}

export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  productSku: string;
  productImageUrl: string | null;
  quantity: number;
  price: number;
  subtotal: number;
}

export interface CreateOrderRequest {
  shippingAddressId?: string;
  shippingAddress?: ShippingAddress;
  billingAddress?: ShippingAddress;
  paymentMethod: string;
  notes?: string;
}
