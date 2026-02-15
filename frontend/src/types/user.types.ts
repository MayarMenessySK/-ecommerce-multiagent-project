export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string | null;
  role: UserRole;
  isActive: boolean;
  isEmailVerified: boolean;
  createdAt: string;
}

export enum UserRole {
  Customer = 'Customer',
  Admin = 'Admin',
  SuperAdmin = 'SuperAdmin'
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  user: User;
}

export interface Address {
  id: string;
  userId: string;
  fullName: string;
  phoneNumber: string;
  addressLine1: string;
  addressLine2: string | null;
  city: string;
  state: string;
  postalCode: string;
  country: string;
  isDefault: boolean;
  addressType: 'Shipping' | 'Billing';
}
