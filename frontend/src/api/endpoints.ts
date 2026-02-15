export const API_ENDPOINTS = {
  // Auth
  AUTH: {
    LOGIN: '/auth/login',
    REGISTER: '/auth/register',
    REFRESH: '/auth/refresh',
    LOGOUT: '/auth/logout',
    FORGOT_PASSWORD: '/auth/forgot-password',
    RESET_PASSWORD: '/auth/reset-password',
  },
  
  // Users
  USERS: {
    PROFILE: '/users/profile',
    UPDATE_PROFILE: '/users/profile',
    CHANGE_PASSWORD: '/users/change-password',
    ADDRESSES: '/users/addresses',
  },
  
  // Products
  PRODUCTS: {
    LIST: '/products',
    DETAIL: (id: string) => `/products/${id}`,
    SEARCH: '/products/search',
  },
  
  // Categories
  CATEGORIES: {
    LIST: '/categories',
    DETAIL: (id: string) => `/categories/${id}`,
  },
  
  // Cart
  CART: {
    GET: '/cart',
    ADD_ITEM: '/cart/items',
    UPDATE_ITEM: (itemId: string) => `/cart/items/${itemId}`,
    REMOVE_ITEM: (itemId: string) => `/cart/items/${itemId}`,
    CLEAR: '/cart/clear',
    APPLY_DISCOUNT: '/cart/discount',
    REMOVE_DISCOUNT: '/cart/discount',
  },
  
  // Orders
  ORDERS: {
    LIST: '/orders',
    CREATE: '/orders',
    DETAIL: (id: string) => `/orders/${id}`,
    CANCEL: (id: string) => `/orders/${id}/cancel`,
  },
  
  // Reviews
  REVIEWS: {
    LIST: (productId: string) => `/reviews/product/${productId}`,
    CREATE: '/reviews',
    UPDATE: (id: string) => `/reviews/${id}`,
    DELETE: (id: string) => `/reviews/${id}`,
  },
  
  // Admin
  ADMIN: {
    DASHBOARD: '/admin/dashboard',
    PRODUCTS: '/admin/products',
    ORDERS: '/admin/orders',
    USERS: '/admin/users',
  },
} as const;
