# E-Commerce API Specification

**Version:** 1.0  
**Date:** December 2024  
**Base URL:** `https://api.ecommerce.example.com/api`  
**Protocol:** HTTPS Only  
**Format:** JSON  
**Authentication:** JWT Bearer Token

---

## Table of Contents
1. [General Information](#general-information)
2. [Authentication Endpoints](#authentication-endpoints)
3. [User Endpoints](#user-endpoints)
4. [Product Endpoints](#product-endpoints)
5. [Category Endpoints](#category-endpoints)
6. [Cart Endpoints](#cart-endpoints)
7. [Order Endpoints](#order-endpoints)
8. [Review Endpoints](#review-endpoints)
9. [Admin Endpoints](#admin-endpoints)
10. [Error Responses](#error-responses)
11. [Rate Limiting](#rate-limiting)

---

## General Information

### API Versioning
- Version included in URL: `/api/v1/...`
- Current version: v1
- Backward compatibility maintained within major versions

### Request Headers
```
Content-Type: application/json
Accept: application/json
Authorization: Bearer {jwt_token}  (for protected endpoints)
Accept-Language: en-US | ar-SA  (optional, defaults to en-US)
X-Request-ID: {unique_request_id}  (optional, for tracking)
```

### Response Format
All responses follow this structure:
```json
{
  "success": true,
  "data": {},
  "message": "Operation successful",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

Error responses:
```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable error message",
    "details": {}
  },
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## Authentication Endpoints

### 1. Register User
**POST** `/api/v1/auth/register`

**Description**: Create a new user account

**Authentication**: None (Public)

**Request Body**:
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "acceptTerms": true
}
```

**Validation Rules**:
- `email`: Required, valid email format, unique
- `password`: Required, min 8 chars, must contain uppercase, lowercase, number, special char
- `firstName`: Required, 2-50 characters
- `lastName`: Required, 2-50 characters
- `phoneNumber`: Optional, valid phone format
- `acceptTerms`: Required, must be true

**Response (201 Created)**:
```json
{
  "success": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "Customer",
    "token": {
      "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresIn": 900,
      "tokenType": "Bearer"
    }
  },
  "message": "Registration successful. Please verify your email.",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Validation errors
- `409 Conflict`: Email already exists
- `429 Too Many Requests`: Rate limit exceeded

---

### 2. Login
**POST** `/api/v1/auth/login`

**Description**: Authenticate user and receive JWT token

**Authentication**: None (Public)

**Request Body**:
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePass123!",
  "rememberMe": false
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "Customer",
    "token": {
      "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresIn": 900,
      "tokenType": "Bearer"
    }
  },
  "message": "Login successful",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Missing required fields
- `401 Unauthorized`: Invalid credentials
- `423 Locked`: Account locked due to too many failed attempts
- `429 Too Many Requests`: Rate limit exceeded

---

### 3. Refresh Token
**POST** `/api/v1/auth/refresh`

**Description**: Refresh expired access token using refresh token

**Authentication**: None (Refresh token in body)

**Request Body**:
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 900,
    "tokenType": "Bearer"
  },
  "message": "Token refreshed successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or expired refresh token

---

### 4. Request Password Reset
**POST** `/api/v1/auth/forgot-password`

**Description**: Request password reset email

**Authentication**: None (Public)

**Request Body**:
```json
{
  "email": "john.doe@example.com"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "If the email exists, a password reset link has been sent.",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Note**: Always returns 200 to prevent email enumeration

---

### 5. Reset Password
**POST** `/api/v1/auth/reset-password`

**Description**: Reset password using token from email

**Authentication**: None (Public)

**Request Body**:
```json
{
  "token": "550e8400-e29b-41d4-a716-446655440000",
  "newPassword": "NewSecurePass123!",
  "confirmPassword": "NewSecurePass123!"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Password reset successful. Please login with your new password.",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid token or passwords don't match
- `410 Gone`: Token expired

---

### 6. Logout
**POST** `/api/v1/auth/logout`

**Description**: Invalidate current session

**Authentication**: Required (Bearer Token)

**Request Body**: None

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Logout successful",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## User Endpoints

### 1. Get Current User Profile
**GET** `/api/v1/users/me`

**Description**: Get authenticated user's profile

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890",
    "profileImageUrl": "https://cdn.example.com/profiles/user123.jpg",
    "role": "Customer",
    "createdAt": "2024-01-01T00:00:00Z",
    "lastLoginAt": "2024-12-01T10:00:00Z",
    "emailVerified": true,
    "addresses": [
      {
        "addressId": "addr-001",
        "type": "Shipping",
        "fullName": "John Doe",
        "addressLine1": "123 Main St",
        "addressLine2": "Apt 4B",
        "city": "New York",
        "state": "NY",
        "postalCode": "10001",
        "country": "USA",
        "phoneNumber": "+1234567890",
        "isDefault": true
      }
    ]
  },
  "message": "Profile retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 2. Update User Profile
**PUT** `/api/v1/users/me`

**Description**: Update authenticated user's profile

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Request Body**:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "profileImageUrl": "https://cdn.example.com/profiles/user123.jpg"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890",
    "profileImageUrl": "https://cdn.example.com/profiles/user123.jpg"
  },
  "message": "Profile updated successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 3. Change Password
**POST** `/api/v1/users/me/change-password`

**Description**: Change user's password

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Request Body**:
```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewSecurePass123!",
  "confirmPassword": "NewSecurePass123!"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Password changed successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Validation error
- `401 Unauthorized`: Current password incorrect

---

### 4. Add Address
**POST** `/api/v1/users/me/addresses`

**Description**: Add shipping/billing address

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Request Body**:
```json
{
  "type": "Shipping",
  "fullName": "John Doe",
  "addressLine1": "123 Main St",
  "addressLine2": "Apt 4B",
  "city": "New York",
  "state": "NY",
  "postalCode": "10001",
  "country": "USA",
  "phoneNumber": "+1234567890",
  "isDefault": false
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "data": {
    "addressId": "addr-002",
    "type": "Shipping",
    "fullName": "John Doe",
    "addressLine1": "123 Main St",
    "addressLine2": "Apt 4B",
    "city": "New York",
    "state": "NY",
    "postalCode": "10001",
    "country": "USA",
    "phoneNumber": "+1234567890",
    "isDefault": false
  },
  "message": "Address added successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 5. Update Address
**PUT** `/api/v1/users/me/addresses/{addressId}`

**Description**: Update existing address

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Path Parameters**:
- `addressId`: Address ID

**Request Body**: Same as Add Address

**Response (200 OK)**: Similar to Add Address response

---

### 6. Delete Address
**DELETE** `/api/v1/users/me/addresses/{addressId}`

**Description**: Delete address

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Address deleted successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## Product Endpoints

### 1. List Products
**GET** `/api/v1/products`

**Description**: Get paginated list of products with filters

**Authentication**: None (Public)

**Query Parameters**:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 20, max: 100)
- `categoryId`: Filter by category ID
- `search`: Search keyword
- `minPrice`: Minimum price
- `maxPrice`: Maximum price
- `minRating`: Minimum rating (1-5)
- `inStock`: Filter in-stock items (true/false)
- `sortBy`: Sort field (featured, price, rating, newest, bestselling)
- `sortOrder`: asc or desc (default: asc)

**Example Request**:
```
GET /api/v1/products?page=1&pageSize=20&categoryId=cat-001&sortBy=price&sortOrder=asc
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "products": [
      {
        "productId": "prod-001",
        "sku": "PROD-12345",
        "name": "Premium Wireless Headphones",
        "slug": "premium-wireless-headphones",
        "description": "High-quality wireless headphones with noise cancellation",
        "shortDescription": "Premium wireless headphones",
        "price": 199.99,
        "originalPrice": 249.99,
        "discountPercentage": 20,
        "currency": "USD",
        "imageUrl": "https://cdn.example.com/products/prod-001-main.jpg",
        "images": [
          "https://cdn.example.com/products/prod-001-1.jpg",
          "https://cdn.example.com/products/prod-001-2.jpg"
        ],
        "categoryId": "cat-001",
        "categoryName": "Electronics",
        "brand": "TechBrand",
        "stockQuantity": 150,
        "inStock": true,
        "averageRating": 4.5,
        "reviewCount": 128,
        "tags": ["wireless", "audio", "premium"],
        "isNew": false,
        "isFeatured": true,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalPages": 5,
      "totalItems": 95,
      "hasNextPage": true,
      "hasPreviousPage": false
    },
    "filters": {
      "appliedFilters": {
        "categoryId": "cat-001",
        "sortBy": "price"
      },
      "priceRange": {
        "min": 19.99,
        "max": 999.99
      }
    }
  },
  "message": "Products retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 2. Get Product Details
**GET** `/api/v1/products/{productId}`

**Description**: Get detailed product information

**Authentication**: None (Public)

**Path Parameters**:
- `productId`: Product ID or slug

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "productId": "prod-001",
    "sku": "PROD-12345",
    "name": "Premium Wireless Headphones",
    "slug": "premium-wireless-headphones",
    "description": "Detailed product description with HTML formatting...",
    "shortDescription": "Premium wireless headphones",
    "price": 199.99,
    "originalPrice": 249.99,
    "discountPercentage": 20,
    "currency": "USD",
    "imageUrl": "https://cdn.example.com/products/prod-001-main.jpg",
    "images": [
      {
        "url": "https://cdn.example.com/products/prod-001-1.jpg",
        "alt": "Front view",
        "isPrimary": true
      },
      {
        "url": "https://cdn.example.com/products/prod-001-2.jpg",
        "alt": "Side view",
        "isPrimary": false
      }
    ],
    "categoryId": "cat-001",
    "categoryName": "Electronics",
    "categoryPath": "Electronics > Audio > Headphones",
    "brand": "TechBrand",
    "stockQuantity": 150,
    "inStock": true,
    "lowStockThreshold": 10,
    "averageRating": 4.5,
    "reviewCount": 128,
    "tags": ["wireless", "audio", "premium"],
    "specifications": [
      {
        "name": "Battery Life",
        "value": "30 hours"
      },
      {
        "name": "Bluetooth Version",
        "value": "5.0"
      },
      {
        "name": "Weight",
        "value": "250g"
      }
    ],
    "variations": [
      {
        "type": "Color",
        "options": [
          {
            "name": "Black",
            "value": "black",
            "available": true,
            "stockQuantity": 80
          },
          {
            "name": "White",
            "value": "white",
            "available": true,
            "stockQuantity": 70
          }
        ]
      }
    ],
    "shipping": {
      "weight": "0.5kg",
      "dimensions": {
        "length": 20,
        "width": 15,
        "height": 10,
        "unit": "cm"
      },
      "freeShippingEligible": true
    },
    "relatedProducts": [
      {
        "productId": "prod-002",
        "name": "Wireless Earbuds",
        "price": 79.99,
        "imageUrl": "https://cdn.example.com/products/prod-002.jpg"
      }
    ],
    "seo": {
      "metaTitle": "Premium Wireless Headphones - TechBrand",
      "metaDescription": "Buy premium wireless headphones with noise cancellation",
      "keywords": ["wireless headphones", "noise cancellation", "audio"]
    },
    "isNew": false,
    "isFeatured": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-11-01T00:00:00Z"
  },
  "message": "Product details retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `404 Not Found`: Product doesn't exist

---

### 3. Search Products
**GET** `/api/v1/products/search`

**Description**: Search products with autocomplete

**Authentication**: None (Public)

**Query Parameters**:
- `q`: Search query (min 3 characters)
- `limit`: Results limit (default: 10, max: 50)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "query": "wireless",
    "suggestions": [
      {
        "productId": "prod-001",
        "name": "Premium Wireless Headphones",
        "price": 199.99,
        "imageUrl": "https://cdn.example.com/products/prod-001.jpg",
        "categoryName": "Electronics"
      }
    ],
    "totalResults": 45
  },
  "message": "Search results retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## Category Endpoints

### 1. List Categories
**GET** `/api/v1/categories`

**Description**: Get hierarchical category list

**Authentication**: None (Public)

**Query Parameters**:
- `includeProducts`: Include product count (default: true)
- `level`: Maximum hierarchy level (default: all)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "categories": [
      {
        "categoryId": "cat-001",
        "name": "Electronics",
        "slug": "electronics",
        "description": "Electronic devices and accessories",
        "imageUrl": "https://cdn.example.com/categories/electronics.jpg",
        "parentCategoryId": null,
        "level": 0,
        "displayOrder": 1,
        "productCount": 150,
        "isActive": true,
        "children": [
          {
            "categoryId": "cat-002",
            "name": "Audio",
            "slug": "audio",
            "description": "Audio devices",
            "imageUrl": "https://cdn.example.com/categories/audio.jpg",
            "parentCategoryId": "cat-001",
            "level": 1,
            "displayOrder": 1,
            "productCount": 45,
            "isActive": true,
            "children": []
          }
        ]
      }
    ]
  },
  "message": "Categories retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 2. Get Category Details
**GET** `/api/v1/categories/{categoryId}`

**Description**: Get category details with products

**Authentication**: None (Public)

**Path Parameters**:
- `categoryId`: Category ID or slug

**Query Parameters**:
- `page`: Page number for products
- `pageSize`: Items per page

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "category": {
      "categoryId": "cat-001",
      "name": "Electronics",
      "slug": "electronics",
      "description": "Electronic devices and accessories",
      "imageUrl": "https://cdn.example.com/categories/electronics.jpg",
      "parentCategoryId": null,
      "breadcrumb": ["Home", "Electronics"],
      "productCount": 150
    },
    "products": [],
    "pagination": {}
  },
  "message": "Category details retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## Cart Endpoints

### 1. Get Cart
**GET** `/api/v1/cart`

**Description**: Get user's shopping cart

**Authentication**: Optional (Guest or Logged-in)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "cartId": "cart-001",
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "items": [
      {
        "cartItemId": "item-001",
        "productId": "prod-001",
        "productName": "Premium Wireless Headphones",
        "sku": "PROD-12345",
        "imageUrl": "https://cdn.example.com/products/prod-001.jpg",
        "price": 199.99,
        "quantity": 2,
        "selectedVariations": {
          "color": "Black"
        },
        "subtotal": 399.98,
        "inStock": true,
        "maxQuantityAvailable": 80
      }
    ],
    "summary": {
      "itemCount": 2,
      "subtotal": 399.98,
      "discount": 0,
      "tax": 35.99,
      "shipping": 0,
      "total": 435.97
    },
    "appliedCoupon": null,
    "createdAt": "2024-12-01T09:00:00Z",
    "updatedAt": "2024-12-01T10:00:00Z"
  },
  "message": "Cart retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 2. Add Item to Cart
**POST** `/api/v1/cart/items`

**Description**: Add product to cart

**Authentication**: Optional (Guest or Logged-in)

**Request Body**:
```json
{
  "productId": "prod-001",
  "quantity": 1,
  "variations": {
    "color": "Black"
  }
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "data": {
    "cartItemId": "item-001",
    "productId": "prod-001",
    "productName": "Premium Wireless Headphones",
    "quantity": 1,
    "price": 199.99,
    "subtotal": 199.99
  },
  "message": "Item added to cart successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid product or quantity
- `409 Conflict`: Insufficient stock

---

### 3. Update Cart Item
**PUT** `/api/v1/cart/items/{cartItemId}`

**Description**: Update item quantity

**Authentication**: Optional (Guest or Logged-in)

**Path Parameters**:
- `cartItemId`: Cart item ID

**Request Body**:
```json
{
  "quantity": 3
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "cartItemId": "item-001",
    "quantity": 3,
    "subtotal": 599.97
  },
  "message": "Cart item updated successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 4. Remove Item from Cart
**DELETE** `/api/v1/cart/items/{cartItemId}`

**Description**: Remove item from cart

**Authentication**: Optional (Guest or Logged-in)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Item removed from cart successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 5. Apply Discount Code
**POST** `/api/v1/cart/apply-coupon`

**Description**: Apply discount code to cart

**Authentication**: Optional (Guest or Logged-in)

**Request Body**:
```json
{
  "couponCode": "SAVE20"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "couponCode": "SAVE20",
    "discountType": "percentage",
    "discountValue": 20,
    "discountAmount": 79.99,
    "newTotal": 355.98
  },
  "message": "Coupon applied successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid or expired coupon
- `422 Unprocessable Entity`: Minimum purchase requirement not met

---

### 6. Remove Discount Code
**DELETE** `/api/v1/cart/remove-coupon`

**Description**: Remove applied discount code

**Authentication**: Optional (Guest or Logged-in)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Coupon removed successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 7. Clear Cart
**DELETE** `/api/v1/cart`

**Description**: Remove all items from cart

**Authentication**: Optional (Guest or Logged-in)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Cart cleared successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## Order Endpoints

### 1. Create Order (Checkout)
**POST** `/api/v1/orders`

**Description**: Place order and process payment

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Request Body**:
```json
{
  "shippingAddressId": "addr-001",
  "billingAddressId": "addr-001",
  "shippingMethod": "standard",
  "paymentMethod": "stripe",
  "paymentDetails": {
    "stripePaymentMethodId": "pm_1234567890"
  },
  "couponCode": "SAVE20",
  "customerNotes": "Please leave at front door"
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "data": {
    "orderId": "order-001",
    "orderNumber": "ORD-2024-001234",
    "status": "Pending",
    "items": [
      {
        "productId": "prod-001",
        "productName": "Premium Wireless Headphones",
        "sku": "PROD-12345",
        "quantity": 2,
        "price": 199.99,
        "subtotal": 399.98
      }
    ],
    "shippingAddress": {},
    "billingAddress": {},
    "summary": {
      "subtotal": 399.98,
      "discount": 79.99,
      "tax": 28.80,
      "shipping": 0,
      "total": 348.79
    },
    "payment": {
      "method": "stripe",
      "status": "Succeeded",
      "transactionId": "pi_1234567890"
    },
    "createdAt": "2024-12-01T10:30:00Z",
    "estimatedDeliveryDate": "2024-12-05T00:00:00Z"
  },
  "message": "Order placed successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Validation error
- `402 Payment Required`: Payment failed
- `409 Conflict`: Insufficient stock

---

### 2. Get Order History
**GET** `/api/v1/orders`

**Description**: Get user's order history

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Query Parameters**:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10)
- `status`: Filter by status (Pending, Processing, Shipped, Delivered, Cancelled)
- `startDate`: Filter from date (ISO 8601)
- `endDate`: Filter to date (ISO 8601)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "orders": [
      {
        "orderId": "order-001",
        "orderNumber": "ORD-2024-001234",
        "status": "Delivered",
        "itemCount": 2,
        "total": 348.79,
        "currency": "USD",
        "createdAt": "2024-12-01T10:30:00Z",
        "deliveredAt": "2024-12-04T15:00:00Z"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalPages": 3,
      "totalItems": 25
    }
  },
  "message": "Orders retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 3. Get Order Details
**GET** `/api/v1/orders/{orderId}`

**Description**: Get detailed order information

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Path Parameters**:
- `orderId`: Order ID

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "orderId": "order-001",
    "orderNumber": "ORD-2024-001234",
    "status": "Delivered",
    "items": [
      {
        "orderItemId": "item-001",
        "productId": "prod-001",
        "productName": "Premium Wireless Headphones",
        "sku": "PROD-12345",
        "imageUrl": "https://cdn.example.com/products/prod-001.jpg",
        "quantity": 2,
        "price": 199.99,
        "subtotal": 399.98,
        "variations": {
          "color": "Black"
        }
      }
    ],
    "shippingAddress": {
      "fullName": "John Doe",
      "addressLine1": "123 Main St",
      "addressLine2": "Apt 4B",
      "city": "New York",
      "state": "NY",
      "postalCode": "10001",
      "country": "USA",
      "phoneNumber": "+1234567890"
    },
    "billingAddress": {},
    "summary": {
      "subtotal": 399.98,
      "discount": 79.99,
      "discountCode": "SAVE20",
      "tax": 28.80,
      "shipping": 0,
      "total": 348.79,
      "currency": "USD"
    },
    "payment": {
      "method": "stripe",
      "status": "Succeeded",
      "last4": "4242",
      "brand": "Visa",
      "transactionId": "pi_1234567890"
    },
    "shipping": {
      "method": "Standard Shipping",
      "carrier": "FedEx",
      "trackingNumber": "1234567890",
      "trackingUrl": "https://fedex.com/track/1234567890",
      "estimatedDeliveryDate": "2024-12-05T00:00:00Z",
      "actualDeliveryDate": "2024-12-04T15:00:00Z"
    },
    "timeline": [
      {
        "status": "Pending",
        "timestamp": "2024-12-01T10:30:00Z",
        "description": "Order placed"
      },
      {
        "status": "Processing",
        "timestamp": "2024-12-01T11:00:00Z",
        "description": "Payment confirmed"
      },
      {
        "status": "Shipped",
        "timestamp": "2024-12-02T09:00:00Z",
        "description": "Package shipped"
      },
      {
        "status": "Delivered",
        "timestamp": "2024-12-04T15:00:00Z",
        "description": "Package delivered"
      }
    ],
    "customerNotes": "Please leave at front door",
    "canCancel": false,
    "canReturn": true,
    "createdAt": "2024-12-01T10:30:00Z",
    "updatedAt": "2024-12-04T15:00:00Z"
  },
  "message": "Order details retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `403 Forbidden`: Cannot access other users' orders
- `404 Not Found`: Order doesn't exist

---

### 4. Cancel Order
**POST** `/api/v1/orders/{orderId}/cancel`

**Description**: Cancel order (if eligible)

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Path Parameters**:
- `orderId`: Order ID

**Request Body**:
```json
{
  "reason": "Changed my mind"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "orderId": "order-001",
    "status": "Cancelled",
    "refundStatus": "Pending",
    "refundAmount": 348.79
  },
  "message": "Order cancelled successfully. Refund will be processed in 3-5 business days.",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Order cannot be cancelled (already shipped/delivered)
- `404 Not Found`: Order doesn't exist

---

### 5. Download Invoice
**GET** `/api/v1/orders/{orderId}/invoice`

**Description**: Download order invoice as PDF

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Path Parameters**:
- `orderId`: Order ID

**Response**: PDF file download

---

## Review Endpoints

### 1. Get Product Reviews
**GET** `/api/v1/products/{productId}/reviews`

**Description**: Get reviews for a product

**Authentication**: None (Public)

**Path Parameters**:
- `productId`: Product ID

**Query Parameters**:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10)
- `rating`: Filter by rating (1-5)
- `sortBy`: newest, highest, lowest, helpful

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "productId": "prod-001",
    "averageRating": 4.5,
    "totalReviews": 128,
    "ratingDistribution": {
      "5": 80,
      "4": 30,
      "3": 10,
      "2": 5,
      "1": 3
    },
    "reviews": [
      {
        "reviewId": "review-001",
        "userId": "user-001",
        "userName": "John D.",
        "rating": 5,
        "title": "Excellent product!",
        "comment": "Best headphones I've ever owned...",
        "verifiedPurchase": true,
        "helpful": 25,
        "notHelpful": 2,
        "images": [
          "https://cdn.example.com/reviews/review-001-1.jpg"
        ],
        "createdAt": "2024-11-15T10:00:00Z",
        "updatedAt": null
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalPages": 13,
      "totalItems": 128
    }
  },
  "message": "Reviews retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 2. Create Review
**POST** `/api/v1/products/{productId}/reviews`

**Description**: Submit product review

**Authentication**: Required (Customer, Admin, SuperAdmin)

**Path Parameters**:
- `productId`: Product ID

**Request Body**:
```json
{
  "rating": 5,
  "title": "Excellent product!",
  "comment": "Best headphones I've ever owned. Great sound quality and comfortable.",
  "images": [
    "https://cdn.example.com/reviews/review-001-1.jpg"
  ]
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "data": {
    "reviewId": "review-002",
    "productId": "prod-001",
    "rating": 5,
    "title": "Excellent product!",
    "comment": "Best headphones I've ever owned. Great sound quality and comfortable.",
    "status": "Pending",
    "createdAt": "2024-12-01T10:30:00Z"
  },
  "message": "Review submitted successfully. It will be published after moderation.",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Validation error
- `403 Forbidden`: Cannot review product (not purchased)
- `409 Conflict`: Already reviewed this product

---

### 3. Update Review
**PUT** `/api/v1/reviews/{reviewId}`

**Description**: Update existing review

**Authentication**: Required (Review owner)

**Path Parameters**:
- `reviewId`: Review ID

**Request Body**: Same as Create Review

**Response (200 OK)**: Similar to Create Review response

---

### 4. Delete Review
**DELETE** `/api/v1/reviews/{reviewId}`

**Description**: Delete review

**Authentication**: Required (Review owner or Admin)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Review deleted successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 5. Mark Review as Helpful
**POST** `/api/v1/reviews/{reviewId}/helpful`

**Description**: Mark review as helpful

**Authentication**: Optional (Logged-in recommended)

**Request Body**:
```json
{
  "isHelpful": true
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "reviewId": "review-001",
    "helpfulCount": 26,
    "notHelpfulCount": 2
  },
  "message": "Thank you for your feedback",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## Admin Endpoints

### 1. Admin Dashboard Stats
**GET** `/api/v1/admin/dashboard`

**Description**: Get dashboard statistics

**Authentication**: Required (Admin, SuperAdmin)

**Query Parameters**:
- `period`: today, week, month, year (default: today)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "sales": {
      "total": 45678.90,
      "currency": "USD",
      "orderCount": 145,
      "averageOrderValue": 315.03,
      "comparedToPrevious": {
        "percentageChange": 12.5,
        "direction": "up"
      }
    },
    "orders": {
      "pending": 12,
      "processing": 45,
      "shipped": 78,
      "delivered": 120,
      "cancelled": 5
    },
    "topProducts": [
      {
        "productId": "prod-001",
        "name": "Premium Wireless Headphones",
        "unitsSold": 45,
        "revenue": 8999.55
      }
    ],
    "recentOrders": [
      {
        "orderId": "order-001",
        "orderNumber": "ORD-2024-001234",
        "customerName": "John Doe",
        "total": 348.79,
        "status": "Processing",
        "createdAt": "2024-12-01T10:30:00Z"
      }
    ],
    "lowStockProducts": [
      {
        "productId": "prod-005",
        "name": "Product Name",
        "stockQuantity": 5,
        "lowStockThreshold": 10
      }
    ],
    "userStats": {
      "totalUsers": 5432,
      "newUsersToday": 23,
      "activeUsers": 1234
    }
  },
  "message": "Dashboard data retrieved successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 2. Admin - List All Orders
**GET** `/api/v1/admin/orders`

**Description**: Get all orders (paginated, filterable)

**Authentication**: Required (Admin, SuperAdmin)

**Query Parameters**:
- `page`, `pageSize`: Pagination
- `status`: Filter by status
- `customerId`: Filter by customer
- `startDate`, `endDate`: Date range
- `search`: Search by order number or customer name

**Response**: Similar to customer order list with all orders

---

### 3. Admin - Update Order Status
**PUT** `/api/v1/admin/orders/{orderId}/status`

**Description**: Update order status

**Authentication**: Required (Admin, SuperAdmin)

**Request Body**:
```json
{
  "status": "Shipped",
  "trackingNumber": "1234567890",
  "carrier": "FedEx",
  "notes": "Package shipped via FedEx"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "orderId": "order-001",
    "status": "Shipped",
    "trackingNumber": "1234567890",
    "carrier": "FedEx"
  },
  "message": "Order status updated successfully. Customer has been notified.",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 4. Admin - Create Product
**POST** `/api/v1/admin/products`

**Description**: Create new product

**Authentication**: Required (Admin, SuperAdmin)

**Request Body**:
```json
{
  "sku": "PROD-12346",
  "name": "New Product",
  "slug": "new-product",
  "description": "Product description...",
  "shortDescription": "Short description",
  "price": 99.99,
  "categoryId": "cat-001",
  "brand": "BrandName",
  "stockQuantity": 100,
  "images": [
    "https://cdn.example.com/products/new-product.jpg"
  ],
  "specifications": [
    {
      "name": "Weight",
      "value": "500g"
    }
  ],
  "tags": ["tag1", "tag2"],
  "isPublished": true,
  "isFeatured": false,
  "seo": {
    "metaTitle": "New Product - Store Name",
    "metaDescription": "Buy new product online"
  }
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "data": {
    "productId": "prod-100",
    "sku": "PROD-12346",
    "name": "New Product",
    "slug": "new-product",
    "price": 99.99,
    "status": "Published",
    "createdAt": "2024-12-01T10:30:00Z"
  },
  "message": "Product created successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 5. Admin - Update Product
**PUT** `/api/v1/admin/products/{productId}`

**Description**: Update existing product

**Authentication**: Required (Admin, SuperAdmin)

**Request Body**: Same as Create Product

**Response (200 OK)**: Similar to Create Product response

---

### 6. Admin - Delete Product
**DELETE** `/api/v1/admin/products/{productId}`

**Description**: Soft delete product

**Authentication**: Required (Admin, SuperAdmin)

**Response (200 OK)**:
```json
{
  "success": true,
  "data": null,
  "message": "Product deleted successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

### 7. Admin - Manage Categories
**POST** `/api/v1/admin/categories`

**Description**: Create new category

**Authentication**: Required (Admin, SuperAdmin)

**Request Body**:
```json
{
  "name": "New Category",
  "slug": "new-category",
  "description": "Category description",
  "parentCategoryId": "cat-001",
  "imageUrl": "https://cdn.example.com/categories/new.jpg",
  "displayOrder": 1,
  "isActive": true
}
```

**Response (201 Created)**: Category object

---

### 8. Admin - Manage Users
**GET** `/api/v1/admin/users`

**Description**: List all users

**Authentication**: Required (SuperAdmin)

**Query Parameters**: Pagination, filtering

**Response**: List of users with details

---

### 9. Admin - Update User Role
**PUT** `/api/v1/admin/users/{userId}/role`

**Description**: Change user role

**Authentication**: Required (SuperAdmin)

**Request Body**:
```json
{
  "role": "Admin"
}
```

**Response (200 OK)**: Updated user object

---

### 10. Admin - Create Discount Code
**POST** `/api/v1/admin/coupons`

**Description**: Create discount code

**Authentication**: Required (Admin, SuperAdmin)

**Request Body**:
```json
{
  "code": "SAVE20",
  "type": "percentage",
  "value": 20,
  "minimumPurchase": 100,
  "maximumDiscount": 50,
  "startDate": "2024-12-01T00:00:00Z",
  "endDate": "2024-12-31T23:59:59Z",
  "usageLimit": 1000,
  "usageLimitPerUser": 1,
  "isActive": true
}
```

**Response (201 Created)**: Coupon object

---

### 11. Admin - Review Moderation
**PUT** `/api/v1/admin/reviews/{reviewId}/moderate`

**Description**: Approve/reject review

**Authentication**: Required (Admin, SuperAdmin)

**Request Body**:
```json
{
  "action": "approve",
  "reason": "Meets guidelines"
}
```

**Response (200 OK)**: Updated review status

---

### 12. Admin - Sales Reports
**GET** `/api/v1/admin/reports/sales`

**Description**: Generate sales report

**Authentication**: Required (Admin, SuperAdmin)

**Query Parameters**:
- `startDate`, `endDate`: Date range
- `groupBy`: day, week, month, product, category
- `format`: json, csv, pdf

**Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "reportType": "sales",
    "period": {
      "startDate": "2024-12-01T00:00:00Z",
      "endDate": "2024-12-31T23:59:59Z"
    },
    "summary": {
      "totalRevenue": 123456.78,
      "totalOrders": 450,
      "averageOrderValue": 274.35,
      "totalItems": 890
    },
    "breakdown": [
      {
        "date": "2024-12-01",
        "revenue": 4567.89,
        "orders": 15,
        "items": 32
      }
    ]
  },
  "message": "Report generated successfully",
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## Error Responses

### Standard Error Format
```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable error message",
    "details": {
      "field": "Specific field error if applicable"
    }
  },
  "timestamp": "2024-12-01T10:30:00Z"
}
```

### Common Error Codes

| Status Code | Error Code | Description |
|-------------|-----------|-------------|
| 400 | `VALIDATION_ERROR` | Request validation failed |
| 400 | `INVALID_REQUEST` | Malformed request |
| 401 | `UNAUTHORIZED` | Missing or invalid authentication token |
| 401 | `INVALID_CREDENTIALS` | Login credentials incorrect |
| 403 | `FORBIDDEN` | Insufficient permissions |
| 404 | `NOT_FOUND` | Resource not found |
| 409 | `CONFLICT` | Resource conflict (e.g., email exists) |
| 409 | `INSUFFICIENT_STOCK` | Product out of stock |
| 410 | `GONE` | Resource no longer available |
| 422 | `UNPROCESSABLE_ENTITY` | Business logic validation failed |
| 423 | `LOCKED` | Account locked |
| 429 | `TOO_MANY_REQUESTS` | Rate limit exceeded |
| 500 | `INTERNAL_ERROR` | Server error |
| 502 | `BAD_GATEWAY` | Upstream service error |
| 503 | `SERVICE_UNAVAILABLE` | Service temporarily unavailable |

### Example Error Responses

**Validation Error (400)**:
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": {
      "email": "Invalid email format",
      "password": "Password must be at least 8 characters"
    }
  },
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Unauthorized (401)**:
```json
{
  "success": false,
  "error": {
    "code": "UNAUTHORIZED",
    "message": "Authentication token is missing or invalid"
  },
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Forbidden (403)**:
```json
{
  "success": false,
  "error": {
    "code": "FORBIDDEN",
    "message": "You don't have permission to access this resource"
  },
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Not Found (404)**:
```json
{
  "success": false,
  "error": {
    "code": "NOT_FOUND",
    "message": "Product not found"
  },
  "timestamp": "2024-12-01T10:30:00Z"
}
```

**Rate Limit (429)**:
```json
{
  "success": false,
  "error": {
    "code": "TOO_MANY_REQUESTS",
    "message": "Rate limit exceeded. Please try again in 60 seconds.",
    "details": {
      "retryAfter": 60,
      "limit": 100,
      "remaining": 0,
      "resetAt": "2024-12-01T10:31:00Z"
    }
  },
  "timestamp": "2024-12-01T10:30:00Z"
}
```

---

## Rate Limiting

### Rate Limit Headers
All API responses include rate limit headers:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1701432600
```

### Limits by Endpoint Type

| Endpoint Type | Authenticated | Anonymous | Window |
|--------------|---------------|-----------|--------|
| Public (GET) | 300/min | 100/min | 1 minute |
| Auth (POST) | - | 10/min | 1 minute |
| User (GET) | 200/min | - | 1 minute |
| User (POST/PUT/DELETE) | 100/min | - | 1 minute |
| Admin (All) | 500/min | - | 1 minute |
| Search | 60/min | 30/min | 1 minute |

### Best Practices
- Implement exponential backoff for retries
- Cache responses when possible
- Use webhooks instead of polling
- Batch requests where supported
- Monitor rate limit headers

---

## Webhooks (Optional)

### Supported Events
- `order.created`
- `order.updated`
- `order.cancelled`
- `payment.succeeded`
- `payment.failed`
- `product.created`
- `product.updated`
- `product.out_of_stock`

### Webhook Payload Example
```json
{
  "eventId": "evt_12345",
  "eventType": "order.created",
  "timestamp": "2024-12-01T10:30:00Z",
  "data": {
    "orderId": "order-001",
    "orderNumber": "ORD-2024-001234",
    "total": 348.79,
    "status": "Pending"
  }
}
```

---

## Pagination

### Query Parameters
- `page`: Page number (1-based, default: 1)
- `pageSize`: Items per page (default: 20, max: 100)

### Response Format
```json
{
  "data": {
    "items": [],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalPages": 5,
      "totalItems": 95,
      "hasNextPage": true,
      "hasPreviousPage": false
    }
  }
}
```

---

## Localization

### Language Header
```
Accept-Language: en-US
Accept-Language: ar-SA
```

### Supported Languages
- `en-US`: English (United States)
- `ar-SA`: Arabic (Saudi Arabia)

### Response
- All text content localized based on Accept-Language header
- Dates formatted according to locale
- Currency displayed according to locale
- RTL support for Arabic

---

## Security

### Authentication
- JWT Bearer tokens in Authorization header
- Access tokens: 15-minute expiry
- Refresh tokens: 7-day expiry

### HTTPS Only
- All requests must use HTTPS
- HTTP requests automatically redirected

### CORS
- Allowed origins configured per environment
- Credentials supported for authenticated requests

### Request Signing (Optional for webhooks)
- HMAC-SHA256 signature in `X-Signature` header

---

## Versioning

### Current Version
- Version: 1.0
- Base URL: `/api/v1`

### Deprecation Policy
- 6-month notice before deprecation
- Old versions supported for 12 months
- Migration guides provided

---

**Document Status**: ✅ Ready for Implementation

**Next Steps**:
1. Backend Engineer: Implement API endpoints
2. Frontend Engineer: Integrate API calls
3. QA Engineer: Test all endpoints
