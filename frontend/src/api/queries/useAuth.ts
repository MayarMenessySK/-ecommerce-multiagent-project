import { useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../api/axios.config';
import { API_ENDPOINTS } from '../api/endpoints';
import { LoginRequest, RegisterRequest, AuthResponse } from '../types/user.types';
import { useAuthStore } from '../store/authStore';

// Login
export const useLogin = () => {
  const { login } = useAuthStore();
  
  return useMutation({
    mutationFn: async (credentials: LoginRequest): Promise<AuthResponse> => {
      return api.post(API_ENDPOINTS.AUTH.LOGIN, credentials);
    },
    onSuccess: (data) => {
      login(data.user, data.token);
      localStorage.setItem('refreshToken', data.refreshToken);
    },
  });
};

// Register
export const useRegister = () => {
  const { login } = useAuthStore();
  
  return useMutation({
    mutationFn: async (data: RegisterRequest): Promise<AuthResponse> => {
      return api.post(API_ENDPOINTS.AUTH.REGISTER, data);
    },
    onSuccess: (data) => {
      login(data.user, data.token);
      localStorage.setItem('refreshToken', data.refreshToken);
    },
  });
};

// Logout
export const useLogout = () => {
  const { logout } = useAuthStore();
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (): Promise<void> => {
      return api.post(API_ENDPOINTS.AUTH.LOGOUT);
    },
    onSuccess: () => {
      logout();
      queryClient.clear();
    },
  });
};
