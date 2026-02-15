import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../axios.config';
import { API_ENDPOINTS } from '../endpoints';
import { 
  Cart, 
  AddToCartRequest, 
  UpdateCartItemRequest,
  ApplyDiscountRequest 
} from '../../types/cart.types';

// Query Keys
export const cartKeys = {
  all: ['cart'] as const,
  detail: () => [...cartKeys.all, 'detail'] as const,
};

// Get Cart
export const useCart = () => {
  return useQuery({
    queryKey: cartKeys.detail(),
    queryFn: async (): Promise<Cart> => {
      return api.get(API_ENDPOINTS.CART.GET);
    },
    staleTime: 0, // Always fetch fresh cart data
  });
};

// Add to Cart
export const useAddToCart = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (request: AddToCartRequest): Promise<Cart> => {
      return api.post(API_ENDPOINTS.CART.ADD_ITEM, request);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all });
    },
  });
};

// Update Cart Item
export const useUpdateCartItem = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (request: UpdateCartItemRequest): Promise<Cart> => {
      return api.put(
        API_ENDPOINTS.CART.UPDATE_ITEM(request.cartItemId),
        { quantity: request.quantity }
      );
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all });
    },
  });
};

// Remove Cart Item
export const useRemoveCartItem = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (cartItemId: string): Promise<Cart> => {
      return api.delete(API_ENDPOINTS.CART.REMOVE_ITEM(cartItemId));
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all });
    },
  });
};

// Clear Cart
export const useClearCart = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (): Promise<void> => {
      return api.delete(API_ENDPOINTS.CART.CLEAR);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all });
    },
  });
};

// Apply Discount Code
export const useApplyDiscount = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (request: ApplyDiscountRequest): Promise<Cart> => {
      return api.post(API_ENDPOINTS.CART.APPLY_DISCOUNT, request);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all });
    },
  });
};

// Remove Discount Code
export const useRemoveDiscount = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (): Promise<Cart> => {
      return api.delete(API_ENDPOINTS.CART.REMOVE_DISCOUNT);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all });
    },
  });
};
