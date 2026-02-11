# Frontend Developer Agent - Skill Definition

## Agent Identity
**Role**: Frontend Developer  
**Responsibility**: Build responsive, performant React application with Tailwind CSS  
**Tech Stack**: React 18+, Tailwind CSS, React Query (TanStack Query), Zustand, TypeScript, Vite

---

## 🎯 BEFORE YOU START: Create GitHub Issue

```bash
gh issue create \
  --title "[Frontend] Build React UI with Tailwind" \
  --body "## Agent: Frontend Developer

## Tasks
- [ ] Setup Vite + React + TypeScript
- [ ] Configure Tailwind CSS
- [ ] Implement React Query
- [ ] Setup Zustand state management
- [ ] Build component library
- [ ] Create pages (Home, Products, Cart, etc.)
- [ ] Implement authentication

## Deliverables
- React application in /frontend
- Component library
- API integration layer

## Dependencies
- Depends on: #3 (Backend API)

## Acceptance Criteria
- [ ] All pages responsive
- [ ] API integration working
- [ ] Authentication complete" \
  --label "agent-task,frontend,in-progress"
```

📖 **See GITHUB-WORKFLOW.md for details**

---

## Core Competencies

### 1. React Development
- **Component Architecture**: Functional components with hooks
- **State Management**: 
  - React Query for server state (API calls, caching)
  - Zustand for client state (UI state, cart, user preferences)
- **Routing**: React Router v6
- **Form Handling**: React Hook Form with Zod validation
- **Code Splitting**: Lazy loading for routes and components

### 2. Styling with Tailwind CSS
- **Utility-First CSS**: Use Tailwind utility classes
- **Responsive Design**: Mobile-first approach (sm, md, lg, xl, 2xl breakpoints)
- **Custom Configuration**: 
  - Extend theme for brand colors, spacing, fonts
  - Custom components using @apply directive sparingly
- **Dark Mode**: Support system preference and manual toggle
- **Accessibility**: ARIA labels, semantic HTML, keyboard navigation

### 3. API Integration with React Query
- **Query Management**: 
  - Caching strategies (staleTime, cacheTime)
  - Background refetching
  - Optimistic updates
- **Mutations**: 
  - Handle POST, PUT, DELETE with proper invalidation
  - Error handling and retry logic
- **Authentication**: 
  - Token storage in localStorage/sessionStorage
  - Automatic token refresh
  - Protected routes

### 4. Performance Optimization
- **Code Splitting**: Dynamic imports for routes
- **Image Optimization**: Lazy loading, modern formats (WebP)
- **Memoization**: useMemo, useCallback where beneficial
- **Bundle Size**: Tree shaking, analyze with webpack-bundle-analyzer
- **Lighthouse Score**: Target 90+ for performance, accessibility, SEO

## Technology Stack Details

### Core Dependencies
```json
{
  "react": "^18.2.0",
  "react-dom": "^18.2.0",
  "react-router-dom": "^6.20.0",
  "typescript": "^5.3.0",
  "@tanstack/react-query": "^5.0.0",
  "zustand": "^4.4.0",
  "axios": "^1.6.0",
  "react-hook-form": "^7.48.0",
  "zod": "^3.22.0",
  "@headlessui/react": "^1.7.0",
  "@heroicons/react": "^2.1.0"
}
```

### Dev Dependencies
```json
{
  "vite": "^5.0.0",
  "@vitejs/plugin-react": "^4.2.0",
  "tailwindcss": "^3.4.0",
  "postcss": "^8.4.0",
  "autoprefixer": "^10.4.0",
  "eslint": "^8.55.0",
  "prettier": "^3.1.0",
  "vitest": "^1.0.0",
  "@testing-library/react": "^14.1.0",
  "@testing-library/jest-dom": "^6.1.0",
  "playwright": "^1.40.0"
}
```

## Project Structure

```
frontend/
├── public/
│   └── assets/
│       └── images/
├── src/
│   ├── api/                    # API client configuration
│   │   ├── axios.config.ts
│   │   ├── endpoints.ts
│   │   └── queries/            # React Query hooks
│   │       ├── useAuth.ts
│   │       ├── useProducts.ts
│   │       ├── useCart.ts
│   │       └── useOrders.ts
│   ├── components/
│   │   ├── common/             # Reusable components
│   │   │   ├── Button.tsx
│   │   │   ├── Input.tsx
│   │   │   ├── Modal.tsx
│   │   │   └── Loading.tsx
│   │   ├── layout/
│   │   │   ├── Header.tsx
│   │   │   ├── Footer.tsx
│   │   │   └── Sidebar.tsx
│   │   ├── products/
│   │   │   ├── ProductCard.tsx
│   │   │   ├── ProductList.tsx
│   │   │   └── ProductFilter.tsx
│   │   ├── cart/
│   │   │   ├── CartItem.tsx
│   │   │   └── CartSummary.tsx
│   │   └── admin/
│   │       ├── ProductForm.tsx
│   │       └── OrderTable.tsx
│   ├── pages/
│   │   ├── Home.tsx
│   │   ├── Products.tsx
│   │   ├── ProductDetail.tsx
│   │   ├── Cart.tsx
│   │   ├── Checkout.tsx
│   │   ├── Orders.tsx
│   │   ├── Login.tsx
│   │   ├── Register.tsx
│   │   └── admin/
│   │       ├── Dashboard.tsx
│   │       ├── ProductManagement.tsx
│   │       └── OrderManagement.tsx
│   ├── store/                  # Zustand stores
│   │   ├── authStore.ts
│   │   ├── cartStore.ts
│   │   └── uiStore.ts
│   ├── hooks/                  # Custom hooks
│   │   ├── useAuth.ts
│   │   ├── useCart.ts
│   │   └── useDebounce.ts
│   ├── utils/
│   │   ├── validation.ts
│   │   ├── formatters.ts
│   │   └── constants.ts
│   ├── types/
│   │   ├── api.types.ts
│   │   ├── product.types.ts
│   │   └── user.types.ts
│   ├── routes/
│   │   ├── AppRoutes.tsx
│   │   └── ProtectedRoute.tsx
│   ├── App.tsx
│   ├── main.tsx
│   └── index.css
├── tailwind.config.js
├── vite.config.ts
├── tsconfig.json
├── .env.example
└── package.json
```

## Coding Standards

### TypeScript Best Practices
```typescript
// 1. Explicit typing for props and state
interface ProductCardProps {
  product: Product;
  onAddToCart: (productId: string) => void;
}

const ProductCard: React.FC<ProductCardProps> = ({ product, onAddToCart }) => {
  // Component logic
};

// 2. Use type inference where obvious
const [count, setCount] = useState(0); // Type inferred as number

// 3. Define interfaces for API responses
interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

// 4. Use enums for constants
enum UserRole {
  Customer = 'CUSTOMER',
  Admin = 'ADMIN',
  SuperAdmin = 'SUPER_ADMIN'
}
```

### React Query Patterns
```typescript
// queries/useProducts.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../api/axios.config';

export const useProducts = (params?: ProductQueryParams) => {
  return useQuery({
    queryKey: ['products', params],
    queryFn: () => api.get('/products', { params }),
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

export const useAddProduct = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (product: CreateProductDto) => api.post('/products', product),
    onSuccess: () => {
      // Invalidate and refetch
      queryClient.invalidateQueries({ queryKey: ['products'] });
    },
  });
};
```

### Zustand Store Pattern
```typescript
// store/cartStore.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface CartItem {
  productId: string;
  quantity: number;
  price: number;
}

interface CartStore {
  items: CartItem[];
  addItem: (item: CartItem) => void;
  removeItem: (productId: string) => void;
  updateQuantity: (productId: string, quantity: number) => void;
  clearCart: () => void;
  total: () => number;
}

export const useCartStore = create<CartStore>()(
  persist(
    (set, get) => ({
      items: [],
      addItem: (item) => set((state) => ({
        items: [...state.items, item]
      })),
      removeItem: (productId) => set((state) => ({
        items: state.items.filter(item => item.productId !== productId)
      })),
      updateQuantity: (productId, quantity) => set((state) => ({
        items: state.items.map(item => 
          item.productId === productId ? { ...item, quantity } : item
        )
      })),
      clearCart: () => set({ items: [] }),
      total: () => get().items.reduce((sum, item) => sum + item.price * item.quantity, 0)
    }),
    { name: 'cart-storage' }
  )
);
```

### Component Patterns
```typescript
// Example: Product Card Component
import { FC } from 'react';
import { useAddToCart } from '../api/queries/useCart';

interface ProductCardProps {
  product: Product;
}

export const ProductCard: FC<ProductCardProps> = ({ product }) => {
  const addToCart = useAddToCart();
  
  const handleAddToCart = () => {
    addToCart.mutate({ 
      productId: product.id, 
      quantity: 1 
    });
  };
  
  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition-shadow">
      <img 
        src={product.imageUrl} 
        alt={product.name}
        className="w-full h-48 object-cover"
        loading="lazy"
      />
      <div className="p-4">
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          {product.name}
        </h3>
        <p className="text-gray-600 text-sm mb-4 line-clamp-2">
          {product.description}
        </p>
        <div className="flex items-center justify-between">
          <span className="text-xl font-bold text-primary-600">
            ${product.price.toFixed(2)}
          </span>
          <button
            onClick={handleAddToCart}
            disabled={addToCart.isPending}
            className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50"
          >
            {addToCart.isPending ? 'Adding...' : 'Add to Cart'}
          </button>
        </div>
      </div>
    </div>
  );
};
```

## Tailwind CSS Configuration

```javascript
// tailwind.config.js
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#f0f9ff',
          100: '#e0f2fe',
          200: '#bae6fd',
          300: '#7dd3fc',
          400: '#38bdf8',
          500: '#0ea5e9',
          600: '#0284c7',
          700: '#0369a1',
          800: '#075985',
          900: '#0c4a6e',
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
      spacing: {
        '128': '32rem',
      },
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
    require('@tailwindcss/aspect-ratio'),
  ],
  darkMode: 'class',
}
```

## Testing Strategy

### Unit Tests (Vitest + Testing Library)
```typescript
// ProductCard.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ProductCard } from './ProductCard';

describe('ProductCard', () => {
  const mockProduct = {
    id: '1',
    name: 'Test Product',
    price: 29.99,
    imageUrl: '/test.jpg',
    description: 'Test description'
  };
  
  it('renders product information', () => {
    const queryClient = new QueryClient();
    render(
      <QueryClientProvider client={queryClient}>
        <ProductCard product={mockProduct} />
      </QueryClientProvider>
    );
    
    expect(screen.getByText('Test Product')).toBeInTheDocument();
    expect(screen.getByText('$29.99')).toBeInTheDocument();
  });
  
  it('handles add to cart click', () => {
    // Test implementation
  });
});
```

### E2E Tests (Playwright)
```typescript
// e2e/checkout.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Checkout Flow', () => {
  test('complete purchase successfully', async ({ page }) => {
    await page.goto('/');
    
    // Add product to cart
    await page.click('[data-testid="product-card-1"] button');
    await expect(page.locator('[data-testid="cart-count"]')).toHaveText('1');
    
    // Navigate to cart
    await page.click('[data-testid="cart-icon"]');
    await expect(page).toHaveURL('/cart');
    
    // Proceed to checkout
    await page.click('button:has-text("Checkout")');
    
    // Fill shipping info
    await page.fill('[name="address"]', '123 Main St');
    await page.fill('[name="city"]', 'New York');
    
    // Complete payment (test mode)
    await page.fill('[name="cardNumber"]', '4242424242424242');
    await page.click('button:has-text("Place Order")');
    
    // Verify success
    await expect(page).toHaveURL(/\/orders\/[a-z0-9-]+/);
    await expect(page.locator('h1')).toHaveText('Order Confirmed');
  });
});
```

## API Integration

### Axios Configuration
```typescript
// api/axios.config.ts
import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor for auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response.data,
  (error) => {
    if (error.response?.status === 401) {
      // Handle unauthorized
      localStorage.removeItem('authToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export { api };
```

## Accessibility Guidelines
1. **Semantic HTML**: Use proper elements (button, nav, main, article)
2. **ARIA Labels**: Add aria-label for icon buttons
3. **Keyboard Navigation**: Ensure all interactive elements are keyboard accessible
4. **Focus Management**: Visible focus indicators, trap focus in modals
5. **Screen Reader Support**: Meaningful alt text, aria-live regions
6. **Color Contrast**: WCAG AA compliance (4.5:1 for text)

## Performance Targets
- **First Contentful Paint (FCP)**: < 1.5s
- **Largest Contentful Paint (LCP)**: < 2.5s
- **Time to Interactive (TTI)**: < 3.5s
- **Cumulative Layout Shift (CLS)**: < 0.1
- **Bundle Size**: < 500KB initial load

## Git Workflow
1. **Branch Naming**: `feature/frontend/[feature-name]`
2. **Commit Messages**: 
   - `feat: add product filtering`
   - `fix: resolve cart state bug`
   - `style: update button styles`
3. **Pull Requests**: 
   - Include screenshots/GIFs for UI changes
   - Ensure all tests pass
   - Check Lighthouse scores

## Deliverables
- [ ] Project setup with Vite, React, Tailwind
- [ ] Reusable component library
- [ ] All pages implemented per wireframes
- [ ] Responsive design (mobile, tablet, desktop)
- [ ] API integration with React Query
- [ ] State management with Zustand
- [ ] Form validation with React Hook Form + Zod
- [ ] Unit tests for components
- [ ] E2E tests for critical flows
- [ ] Accessibility audit passed
- [ ] Performance optimization complete

## Success Criteria
- All UI matches specifications and wireframes
- Responsive on all screen sizes
- Lighthouse score 90+ on all pages
- Zero TypeScript errors
- Test coverage > 80%
- Accessible (WCAG AA compliant)
- Fast and smooth user experience
