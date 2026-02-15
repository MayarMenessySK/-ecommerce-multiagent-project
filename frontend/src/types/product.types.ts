export interface Product {
  id: string;
  name: string;
  slug: string;
  sku: string;
  description: string | null;
  shortDescription: string | null;
  price: number;
  originalPrice: number | null;
  discountPercentage: number | null;
  currency: string;
  stockQuantity: number;
  categoryId: string;
  brand: string | null;
  isFeatured: boolean;
  isActive: boolean;
  averageRating: number;
  totalReviews: number;
  images: ProductImage[];
  category: Category | null;
}

export interface ProductImage {
  id: string;
  imageUrl: string;
  altText: string | null;
  isPrimary: boolean;
  displayOrder: number;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  description: string | null;
  imageUrl: string | null;
  parentCategoryId: string | null;
  level: number;
  isActive: boolean;
}

export interface ProductListItem {
  id: string;
  name: string;
  slug: string;
  price: number;
  originalPrice: number | null;
  stockQuantity: number;
  isFeatured: boolean;
  averageRating: number;
  totalReviews: number;
  primaryImage: string | null;
  categoryName: string;
}

export interface ProductFilters {
  categoryId?: string;
  searchQuery?: string;
  minPrice?: number;
  maxPrice?: number;
  brand?: string;
  isAvailable?: boolean;
  isFeatured?: boolean;
  sortBy?: 'price_asc' | 'price_desc' | 'name_asc' | 'name_desc' | 'newest' | 'rating';
  page?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
