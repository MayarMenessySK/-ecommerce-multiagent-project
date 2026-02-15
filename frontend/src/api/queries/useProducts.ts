import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../axios.config';
import { API_ENDPOINTS } from '../endpoints';
import { Product, ProductListItem, ProductFilters, PagedResult } from '../../types/product.types';

// Query Keys
export const productKeys = {
  all: ['products'] as const,
  lists: () => [...productKeys.all, 'list'] as const,
  list: (filters: ProductFilters) => [...productKeys.lists(), filters] as const,
  details: () => [...productKeys.all, 'detail'] as const,
  detail: (id: string) => [...productKeys.details(), id] as const,
};

// Get Products List with Filters
export const useProducts = (filters: ProductFilters = {}) => {
  return useQuery({
    queryKey: productKeys.list(filters),
    queryFn: async (): Promise<PagedResult<ProductListItem>> => {
      return api.get(API_ENDPOINTS.PRODUCTS.LIST, { params: filters });
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

// Get Product Detail
export const useProduct = (id: string) => {
  return useQuery({
    queryKey: productKeys.detail(id),
    queryFn: async (): Promise<Product> => {
      return api.get(API_ENDPOINTS.PRODUCTS.DETAIL(id));
    },
    enabled: !!id,
  });
};

// Search Products
export const useProductSearch = (searchQuery: string) => {
  return useQuery({
    queryKey: ['products', 'search', searchQuery],
    queryFn: async (): Promise<ProductListItem[]> => {
      return api.get(API_ENDPOINTS.PRODUCTS.SEARCH, {
        params: { query: searchQuery },
      });
    },
    enabled: searchQuery.length > 2,
    staleTime: 30 * 1000, // 30 seconds
  });
};

// Featured Products
export const useFeaturedProducts = () => {
  return useProducts({ isFeatured: true, pageSize: 8 });
};
