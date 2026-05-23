# 📱 Documentação Frontend - Autenticação ZetaFin

## Guia Completo para React (Web + Mobile)

---

## 🎯 Objetivo

Implementar autenticação segura no ZetaFin frontend com suporte para:
- ✅ Web (React responsivo)
- ✅ Mobile (React Native - futuro)
- ✅ Logout seguro
- ✅ Token refresh automático
- ✅ Sessão por dispositivo
- ✅ Armazenamento seguro

---

## 📋 Índice

1. [Arquitetura de Autenticação](#arquitetura)
2. [Tipos & Interfaces TypeScript](#tipos)
3. [API Integration (Axios/Fetch)](#api-integration)
4. [Context API para Autenticação](#context-api)
5. [Hooks Customizados](#hooks-customizados)
6. [Componentes de Exemplo](#componentes)
7. [Fluxo de Login](#fluxo-login)
8. [Fluxo de Registro](#fluxo-registro)
9. [Dashboard com Dados do Usuário](#dashboard)
10. [Tratamento de Erros](#tratamento-erros)
11. [Mobile (React Native) - Diferenças](#mobile-react-native)
12. [Segurança](#segurança)

---

## 🏗️ Arquitetura de Autenticação {#arquitetura}

```
┌─────────────────────────────────────────────────────┐
│             FRONTEND (React)                        │
├─────────────────────────────────────────────────────┤
│ ┌──────────────────────────────────────────────┐   │
│ │  AuthContext + useAuth Hook                  │   │
│ │  - isAuthenticated                           │   │
│ │  - user (dados do usuário)                   │   │
│ │  - loading                                   │   │
│ │  - error                                     │   │
│ └──────────────────────────────────────────────┘   │
│                     │                               │
│                     ▼                               │
│ ┌──────────────────────────────────────────────┐   │
│ │  AuthService                                 │   │
│ │  - register()                                │   │
│ │  - login()                                   │   │
│ │  - logout()                                  │   │
│ │  - refreshToken()                            │   │
│ └──────────────────────────────────────────────┘   │
│                     │                               │
│                     ▼                               │
│ ┌──────────────────────────────────────────────┐   │
│ │  API Client (Axios)                          │   │
│ │  - Interceptors                              │   │
│ │  - JWT Header                                │   │
│ │  - Retry Logic                               │   │
│ └──────────────────────────────────────────────┘   │
│                     │                               │
│                     ▼                               │
│ ┌──────────────────────────────────────────────┐   │
│ │  Local/Session Storage (Web)                 │   │
│ │  Keychain/Keystore (Mobile)                  │   │
│ └──────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
                      │
                      │ HTTPS
                      ▼
        ┌────────────────────────────┐
        │   ZetaFin API Backend      │
        │   Endpoints:               │
        │   - /auth/register         │
        │   - /auth/login            │
        │   - /auth/refresh          │
        │   - /auth/logout           │
        │   - /sessions/active       │
        └────────────────────────────┘
```

---

## 📝 Tipos & Interfaces TypeScript {#tipos}

### 1. User Types
```typescript
// src/types/auth.ts

export interface User {
  id: string;
  name: string;
  email: string;
  role: 'User' | 'Admin' | 'Support' | 'FamilyOwner' | 'FamilyAdult' | 'FamilyDependent';
  isEmailConfirmed: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  userId: string;
  name: string;
  email: string;
  role: string;
  accessTokenExpiresIn: number; // em segundos
  tokenType: string; // "Bearer"
}

export interface LoginRequest {
  email: string;
  password: string;
  deviceName?: string;
  deviceType?: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  passwordConfirmation: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  newPasswordConfirmation: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  email: string;
  newPassword: string;
  newPasswordConfirmation: string;
}

export interface UserSession {
  id: string;
  deviceName: string;
  deviceType: string;
  ipAddress: string;
  createdAt: string;
  lastAccessAt: string;
  isActive: boolean;
  isCurrentSession?: boolean;
}

export interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  login: (credentials: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  logoutAll: () => Promise<void>;
  changePassword: (data: ChangePasswordRequest) => Promise<void>;
  forgotPassword: (data: ForgotPasswordRequest) => Promise<void>;
  resetPassword: (data: ResetPasswordRequest) => Promise<void>;
  getSessions: () => Promise<UserSession[]>;
  terminateSession: (sessionId: string) => Promise<void>;
  clearError: () => void;
}
```

### 2. API Response Types
```typescript
// src/types/api.ts

export interface ApiError {
  error?: string;
  message?: string;
  details?: Record<string, any>;
  statusCode?: number;
}

export interface ApiResponse<T> {
  data?: T;
  error?: string;
  success: boolean;
}
```

---

## 🔗 API Integration {#api-integration}

### 1. Axios Client
```typescript
// src/services/api/client.ts

import axios, { AxiosInstance, AxiosError } from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5001/api';
const API_TIMEOUT = 30000; // 30 segundos

export class ApiClient {
  private client: AxiosInstance;
  private static instance: ApiClient;

  private constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      timeout: API_TIMEOUT,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request Interceptor - Adicionar token
    this.client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('accessToken');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }

        // Adicionar device info para rastreamento
        const deviceName = this.getDeviceName();
        const deviceType = this.getDeviceType();

        config.headers['X-Device-Name'] = deviceName;
        config.headers['X-Device-Type'] = deviceType;

        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response Interceptor - Renovar token
    this.client.interceptors.response.use(
      (response) => response,
      async (error: AxiosError) => {
        const originalRequest = error.config as any;

        // Se 401 e não é tentativa de refresh, renovar token
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;

          try {
            const refreshToken = localStorage.getItem('refreshToken');
            if (!refreshToken) {
              throw new Error('No refresh token available');
            }

            const response = await this.client.post('/authentication/refresh', {
              refreshToken,
              deviceName: this.getDeviceName(),
            });

            const { accessToken, refreshToken: newRefreshToken } = response.data;

            localStorage.setItem('accessToken', accessToken);
            localStorage.setItem('refreshToken', newRefreshToken);

            // Retry original request com novo token
            originalRequest.headers.Authorization = `Bearer ${accessToken}`;
            return this.client(originalRequest);
          } catch (refreshError) {
            // Falha no refresh - fazer logout
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            window.location.href = '/login';
            return Promise.reject(refreshError);
          }
        }

        return Promise.reject(error);
      }
    );
  }

  static getInstance(): ApiClient {
    if (!ApiClient.instance) {
      ApiClient.instance = new ApiClient();
    }
    return ApiClient.instance;
  }

  private getDeviceName(): string {
    if (typeof window !== 'undefined' && navigator.userAgent) {
      const ua = navigator.userAgent;
      if (ua.includes('Chrome')) return 'Chrome';
      if (ua.includes('Firefox')) return 'Firefox';
      if (ua.includes('Safari')) return 'Safari';
      if (ua.includes('Edge')) return 'Edge';
      return 'Web Browser';
    }
    return 'Unknown Device';
  }

  private getDeviceType(): string {
    return 'Web'; // Mobile será "Mobile" no React Native
  }

  async get<T>(url: string, config?: any): Promise<T> {
    const response = await this.client.get<T>(url, config);
    return response.data;
  }

  async post<T>(url: string, data?: any, config?: any): Promise<T> {
    const response = await this.client.post<T>(url, data, config);
    return response.data;
  }

  async put<T>(url: string, data?: any, config?: any): Promise<T> {
    const response = await this.client.put<T>(url, data, config);
    return response.data;
  }

  async delete<T>(url: string, config?: any): Promise<T> {
    const response = await this.client.delete<T>(url, config);
    return response.data;
  }

  async patch<T>(url: string, data?: any, config?: any): Promise<T> {
    const response = await this.client.patch<T>(url, data, config);
    return response.data;
  }
}

export const apiClient = ApiClient.getInstance();
```

### 2. Auth Service
```typescript
// src/services/api/authService.ts

import { apiClient } from './client';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  ChangePasswordRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  User,
  UserSession,
} from '../../types/auth';

export class AuthService {
  static async register(data: RegisterRequest): Promise<AuthResponse> {
    return apiClient.post('/authentication/register', data);
  }

  static async login(credentials: LoginRequest): Promise<AuthResponse> {
    return apiClient.post('/authentication/login', credentials);
  }

  static async refresh(refreshToken: string, deviceName?: string): Promise<AuthResponse> {
    return apiClient.post('/authentication/refresh', {
      refreshToken,
      deviceName,
    });
  }

  static async logout(): Promise<void> {
    try {
      await apiClient.post('/authentication/logout', {});
    } finally {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
    }
  }

  static async logoutAll(): Promise<void> {
    try {
      await apiClient.post('/authentication/logout-all', {});
    } finally {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
    }
  }

  static async changePassword(data: ChangePasswordRequest): Promise<void> {
    await apiClient.post('/authentication/change-password', data);
  }

  static async forgotPassword(data: ForgotPasswordRequest): Promise<void> {
    await apiClient.post('/authentication/forgot-password', data);
  }

  static async resetPassword(data: ResetPasswordRequest): Promise<void> {
    await apiClient.post('/authentication/reset-password', data);
  }

  static async getSessions(): Promise<UserSession[]> {
    return apiClient.get('/sessions/active');
  }

  static async terminateSession(sessionId: string): Promise<void> {
    await apiClient.delete(`/sessions/${sessionId}`);
  }

  static async terminateAllSessions(): Promise<void> {
    await apiClient.delete('/sessions/all');
  }

  static getStoredTokens() {
    return {
      accessToken: localStorage.getItem('accessToken'),
      refreshToken: localStorage.getItem('refreshToken'),
    };
  }

  static setTokens(accessToken: string, refreshToken: string) {
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
  }

  static clearTokens() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  static isTokenExpired(token: string): boolean {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      const decoded = JSON.parse(jsonPayload);
      return Date.now() >= decoded.exp * 1000;
    } catch {
      return true;
    }
  }

  static getUserFromToken(token: string): Partial<User> | null {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      const decoded = JSON.parse(jsonPayload);
      return {
        id: decoded.sub,
        email: decoded.email,
        name: decoded.name || decoded.unique_name,
        role: decoded.role,
      };
    } catch {
      return null;
    }
  }
}
```

---

## 🎯 Context API para Autenticação {#context-api}

### 1. AuthContext
```typescript
// src/contexts/AuthContext.tsx

import React, { createContext, useCallback, useEffect, useState } from 'react';
import { AuthService } from '../services/api/authService';
import { AuthContextType, AuthResponse, User } from '../types/auth';

export const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Inicializar usuário a partir do token armazenado
  useEffect(() => {
    const initializeAuth = async () => {
      try {
        const { accessToken } = AuthService.getStoredTokens();

        if (!accessToken) {
          setIsLoading(false);
          return;
        }

        // Verificar se token está expirado
        if (AuthService.isTokenExpired(accessToken)) {
          AuthService.clearTokens();
          setIsLoading(false);
          return;
        }

        // Extrair user do token JWT
        const userFromToken = AuthService.getUserFromToken(accessToken);
        if (userFromToken) {
          setUser({
            ...userFromToken,
            id: userFromToken.id || '',
            name: userFromToken.name || '',
            email: userFromToken.email || '',
            role: userFromToken.role || 'User',
            isEmailConfirmed: true,
            createdAt: new Date().toISOString(),
          });
        }
      } catch (err) {
        console.error('Error initializing auth:', err);
        AuthService.clearTokens();
      } finally {
        setIsLoading(false);
      }
    };

    initializeAuth();
  }, []);

  const login = useCallback(async (credentials) => {
    setIsLoading(true);
    setError(null);

    try {
      const response: AuthResponse = await AuthService.login({
        ...credentials,
        deviceName: navigator.userAgent.split('/')[0],
        deviceType: 'Web',
      });

      AuthService.setTokens(response.accessToken, response.refreshToken);

      setUser({
        id: response.userId,
        name: response.name,
        email: response.email,
        role: response.role,
        isEmailConfirmed: true,
        createdAt: new Date().toISOString(),
      });
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || 'Erro ao fazer login';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const register = useCallback(async (data) => {
    setIsLoading(true);
    setError(null);

    try {
      const response: AuthResponse = await AuthService.register(data);

      AuthService.setTokens(response.accessToken, response.refreshToken);

      setUser({
        id: response.userId,
        name: response.name,
        email: response.email,
        role: response.role,
        isEmailConfirmed: false,
        createdAt: new Date().toISOString(),
      });
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || 'Erro ao registrar';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const logout = useCallback(async () => {
    setIsLoading(true);
    try {
      await AuthService.logout();
      setUser(null);
    } catch (err) {
      console.error('Error logging out:', err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const logoutAll = useCallback(async () => {
    setIsLoading(true);
    try {
      await AuthService.logoutAll();
      setUser(null);
    } catch (err) {
      console.error('Error logging out from all sessions:', err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const changePassword = useCallback(async (data) => {
    setIsLoading(true);
    setError(null);

    try {
      await AuthService.changePassword(data);
      // Após mudar senha, fazer logout (por segurança)
      await logout();
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || 'Erro ao mudar senha';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, [logout]);

  const forgotPassword = useCallback(async (data) => {
    setIsLoading(true);
    setError(null);

    try {
      await AuthService.forgotPassword(data);
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || 'Erro ao solicitar reset';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const resetPassword = useCallback(async (data) => {
    setIsLoading(true);
    setError(null);

    try {
      await AuthService.resetPassword(data);
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || 'Erro ao fazer reset';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const getSessions = useCallback(async () => {
    try {
      return await AuthService.getSessions();
    } catch (err) {
      console.error('Error getting sessions:', err);
      return [];
    }
  }, []);

  const terminateSession = useCallback(async (sessionId: string) => {
    setIsLoading(true);
    try {
      await AuthService.terminateSession(sessionId);
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || 'Erro ao encerrar sessão';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    error,
    login,
    register,
    logout,
    logoutAll,
    changePassword,
    forgotPassword,
    resetPassword,
    getSessions,
    terminateSession,
    clearError,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
```

---

## 🪝 Hooks Customizados {#hooks-customizados}

### 1. useAuth Hook
```typescript
// src/hooks/useAuth.ts

import { useContext } from 'react';
import { AuthContext } from '../contexts/AuthContext';
import { AuthContextType } from '../types/auth';

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth deve ser usado dentro de AuthProvider');
  }

  return context;
};
```

### 2. useRequireAuth Hook (Para Rotas Protegidas)
```typescript
// src/hooks/useRequireAuth.ts

import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './useAuth';

export const useRequireAuth = () => {
  const navigate = useNavigate();
  const { isAuthenticated, isLoading } = useAuth();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      navigate('/login', { replace: true });
    }
  }, [isAuthenticated, isLoading, navigate]);

  return { isAuthenticated, isLoading };
};
```

### 3. useSessions Hook (Para Gerenciar Sessões)
```typescript
// src/hooks/useSessions.ts

import { useState, useCallback } from 'react';
import { useAuth } from './useAuth';
import { UserSession } from '../types/auth';

export const useSessions = () => {
  const { getSessions, terminateSession } = useAuth();
  const [sessions, setSessions] = useState<UserSession[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchSessions = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const data = await getSessions();
      setSessions(data);
    } catch (err: any) {
      setError(err.message || 'Erro ao buscar sessões');
    } finally {
      setIsLoading(false);
    }
  }, [getSessions]);

  const endSession = useCallback(
    async (sessionId: string) => {
      setIsLoading(true);
      setError(null);

      try {
        await terminateSession(sessionId);
        setSessions((prev) => prev.filter((s) => s.id !== sessionId));
      } catch (err: any) {
        setError(err.message || 'Erro ao encerrar sessão');
      } finally {
        setIsLoading(false);
      }
    },
    [terminateSession]
  );

  return { sessions, isLoading, error, fetchSessions, endSession };
};
```

---

## 🎨 Componentes de Exemplo {#componentes}

### 1. Login Component
```typescript
// src/pages/Login.tsx

import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import './Login.css';

export const Login: React.FC = () => {
  const navigate = useNavigate();
  const { login, isLoading, error, clearError } = useAuth();

  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });

  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!formData.email) {
      errors.email = 'Email é obrigatório';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      errors.email = 'Email inválido';
    }

    if (!formData.password) {
      errors.password = 'Senha é obrigatória';
    } else if (formData.password.length < 6) {
      errors.password = 'Senha deve ter no mínimo 6 caracteres';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));

    // Limpar erro do campo
    if (validationErrors[name]) {
      setValidationErrors((prev) => ({
        ...prev,
        [name]: '',
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    clearError();

    if (!validateForm()) {
      return;
    }

    try {
      await login({
        email: formData.email,
        password: formData.password,
      });

      navigate('/dashboard');
    } catch (err) {
      // Erro já está no context
      console.error('Login error:', err);
    }
  };

  return (
    <div className="login-container">
      <div className="login-box">
        <h1>ZetaFin</h1>
        <h2>Fazer Login</h2>

        {error && <div className="alert alert-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="seu@email.com"
              disabled={isLoading}
              className={validationErrors.email ? 'error' : ''}
            />
            {validationErrors.email && (
              <span className="error-message">{validationErrors.email}</span>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="password">Senha</label>
            <input
              id="password"
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="••••••••"
              disabled={isLoading}
              className={validationErrors.password ? 'error' : ''}
            />
            {validationErrors.password && (
              <span className="error-message">{validationErrors.password}</span>
            )}
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="btn-login"
          >
            {isLoading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="login-footer">
          <p>Não tem conta? <a href="/register">Registre-se</a></p>
          <p><a href="/forgot-password">Esqueceu a senha?</a></p>
        </div>
      </div>
    </div>
  );
};
```

### 2. Register Component
```typescript
// src/pages/Register.tsx

import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import './Register.css';

export const Register: React.FC = () => {
  const navigate = useNavigate();
  const { register, isLoading, error, clearError } = useAuth();

  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '',
    passwordConfirmation: '',
  });

  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});

  const validatePassword = (password: string): string[] => {
    const errors: string[] = [];

    if (password.length < 8) {
      errors.push('Mínimo 8 caracteres');
    }
    if (!/[A-Z]/.test(password)) {
      errors.push('Pelo menos 1 letra maiúscula');
    }
    if (!/[a-z]/.test(password)) {
      errors.push('Pelo menos 1 letra minúscula');
    }
    if (!/[0-9]/.test(password)) {
      errors.push('Pelo menos 1 número');
    }
    if (!/[!@#$%^&*]/.test(password)) {
      errors.push('Pelo menos 1 caractere especial (!@#$%^&*)');
    }

    return errors;
  };

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!formData.name.trim()) {
      errors.name = 'Nome é obrigatório';
    }

    if (!formData.email) {
      errors.email = 'Email é obrigatório';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      errors.email = 'Email inválido';
    }

    const passwordErrors = validatePassword(formData.password);
    if (passwordErrors.length > 0) {
      errors.password = passwordErrors.join(', ');
    }

    if (formData.password !== formData.passwordConfirmation) {
      errors.passwordConfirmation = 'As senhas não correspondem';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));

    if (validationErrors[name]) {
      setValidationErrors((prev) => ({
        ...prev,
        [name]: '',
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    clearError();

    if (!validateForm()) {
      return;
    }

    try {
      await register({
        name: formData.name,
        email: formData.email,
        password: formData.password,
        passwordConfirmation: formData.passwordConfirmation,
      });

      // Redirecionar para dashboard após sucesso
      navigate('/dashboard');
    } catch (err) {
      console.error('Register error:', err);
    }
  };

  return (
    <div className="register-container">
      <div className="register-box">
        <h1>ZetaFin</h1>
        <h2>Criar Conta</h2>

        {error && <div className="alert alert-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="name">Nome Completo</label>
            <input
              id="name"
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              placeholder="Seu Nome"
              disabled={isLoading}
              className={validationErrors.name ? 'error' : ''}
            />
            {validationErrors.name && (
              <span className="error-message">{validationErrors.name}</span>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="seu@email.com"
              disabled={isLoading}
              className={validationErrors.email ? 'error' : ''}
            />
            {validationErrors.email && (
              <span className="error-message">{validationErrors.email}</span>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="password">Senha</label>
            <input
              id="password"
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="••••••••"
              disabled={isLoading}
              className={validationErrors.password ? 'error' : ''}
            />
            {validationErrors.password && (
              <span className="error-message">{validationErrors.password}</span>
            )}
            <small>Mínimo 8 caracteres, com maiúscula, minúscula, número e caractere especial</small>
          </div>

          <div className="form-group">
            <label htmlFor="passwordConfirmation">Confirmar Senha</label>
            <input
              id="passwordConfirmation"
              type="password"
              name="passwordConfirmation"
              value={formData.passwordConfirmation}
              onChange={handleChange}
              placeholder="••••••••"
              disabled={isLoading}
              className={validationErrors.passwordConfirmation ? 'error' : ''}
            />
            {validationErrors.passwordConfirmation && (
              <span className="error-message">{validationErrors.passwordConfirmation}</span>
            )}
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="btn-register"
          >
            {isLoading ? 'Criando conta...' : 'Criar Conta'}
          </button>
        </form>

        <div className="register-footer">
          <p>Já tem conta? <a href="/login">Faça login</a></p>
        </div>
      </div>
    </div>
  );
};
```

### 3. Dashboard Component
```typescript
// src/pages/Dashboard.tsx

import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { useRequireAuth } from '../hooks/useRequireAuth';
import { useSessions } from '../hooks/useSessions';
import { UserSession } from '../types/auth';
import './Dashboard.css';

export const Dashboard: React.FC = () => {
  const navigate = useNavigate();
  const { user, logout, logoutAll } = useAuth();
  const { isAuthenticated, isLoading } = useRequireAuth();
  const { sessions, fetchSessions } = useSessions();

  const [showLogoutMenu, setShowLogoutMenu] = useState(false);

  useEffect(() => {
    if (isAuthenticated && !isLoading) {
      fetchSessions();
    }
  }, [isAuthenticated, isLoading, fetchSessions]);

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const handleLogoutAll = async () => {
    await logoutAll();
    navigate('/login');
  };

  if (isLoading) {
    return <div className="loading">Carregando...</div>;
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  return (
    <div className="dashboard">
      {/* Header */}
      <header className="dashboard-header">
        <div className="header-content">
          <div className="logo">
            <h1>ZetaFin</h1>
          </div>

          <nav className="nav-items">
            <a href="/dashboard">Dashboard</a>
            <a href="/settings">Configurações</a>
          </nav>

          <div className="user-menu">
            <button
              className="user-button"
              onClick={() => setShowLogoutMenu(!showLogoutMenu)}
            >
              <span className="user-name">{user.name}</span>
              <span className="user-initial">{user.name.charAt(0).toUpperCase()}</span>
            </button>

            {showLogoutMenu && (
              <div className="dropdown-menu">
                <a href="/profile">Meu Perfil</a>
                <a href="/security">Segurança</a>
                <button onClick={handleLogout}>Fazer Logout</button>
                <button onClick={handleLogoutAll} className="logout-all">
                  Logout de Todos os Dispositivos
                </button>
              </div>
            )}
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="dashboard-content">
        <div className="welcome-section">
          <h2>Bem-vindo, {user.name}! 👋</h2>
          <p className="user-email">{user.email}</p>
        </div>

        {/* Sessões Ativas */}
        <section className="sessions-section">
          <h3>Dispositivos Conectados</h3>
          <div className="sessions-list">
            {sessions.length > 0 ? (
              sessions.map((session: UserSession) => (
                <div key={session.id} className="session-card">
                  <div className="session-info">
                    <div className="session-device">
                      <strong>{session.deviceName}</strong>
                      <span className="device-type">{session.deviceType}</span>
                    </div>
                    <div className="session-details">
                      <small>IP: {session.ipAddress}</small>
                      <small>Último acesso: {new Date(session.lastAccessAt).toLocaleString()}</small>
                    </div>
                  </div>
                  {!session.isCurrentSession && (
                    <button className="btn-terminate">
                      Encerrar
                    </button>
                  )}
                </div>
              ))
            ) : (
              <p>Nenhuma sessão ativa</p>
            )}
          </div>
        </section>

        {/* Cards de Funcionalidades */}
        <section className="features-section">
          <div className="feature-card">
            <h3>💰 Contas</h3>
            <p>Gerencie suas contas e carteiras</p>
            <button>Acessar</button>
          </div>

          <div className="feature-card">
            <h3>🎯 Metas</h3>
            <p>Defina e acompanhe suas metas financeiras</p>
            <button>Acessar</button>
          </div>

          <div className="feature-card">
            <h3>📊 Relatórios</h3>
            <p>Veja análises detalhadas de suas finanças</p>
            <button>Acessar</button>
          </div>

          <div className="feature-card">
            <h3>⚙️ Configurações</h3>
            <p>Personalize sua experiência</p>
            <button onClick={() => navigate('/settings')}>Acessar</button>
          </div>
        </section>
      </main>
    </div>
  );
};
```

---

## 🔄 Fluxo de Login {#fluxo-login}

```
┌─────────────────────────────────────────────────┐
│ 1. PÁGINA DE LOGIN                              │
│    - Usuario digita email + senha               │
│    - Form validation (client-side)              │
└──────────────────┬──────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────┐
│ 2. SUBMIT FORM                                  │
│    - Validar dados                              │
│    - Desabilitar botão                          │
│    - Mostrar loading                            │
└──────────────────┬──────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────┐
│ 3. API CALL - POST /auth/login                  │
│    - Enviar email + password                    │
│    - Device info no header                      │
│    - Timeout: 30s                               │
└──────────────────┬──────────────────────────────┘
                   │
         ┌─────────┴────────────┐
         │                      │
         ▼                      ▼
    ✅ SUCESSO              ❌ ERRO
    (200 OK)               (401 Unauthorized)
         │                      │
         ▼                      ▼
┌──────────────────┐    ┌──────────────────────┐
│ 4. RECEBER TOKEN │    │ MOSTRAR ERROR MSG    │
│ - accessToken    │    │ - "Credenciais      │
│ - refreshToken   │    │    inválidas"       │
│ - user data      │    │ - Limpar formulário  │
└────────┬─────────┘    │ - Manter na página   │
         │              └──────────────────────┘
         ▼
┌─────────────────────────────────────────────────┐
│ 5. ARMAZENAR TOKENS                             │
│ localStorage.setItem('accessToken', token)      │
│ localStorage.setItem('refreshToken', token)     │
│                                                 │
│ 🔐 Web: localStorage                            │
│ 🔐 Mobile: SecureStore / Keychain / Keystore    │
└────────┬─────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────┐
│ 6. ATUALIZAR STATE                              │
│ - setUser() com dados do usuário                │
│ - setIsAuthenticated(true)                      │
│ - Limpar erros                                  │
└────────┬─────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────┐
│ 7. REDIRECT                                     │
│ navigate('/dashboard')                          │
└─────────────────────────────────────────────────┘
```

---

## 📝 Fluxo de Registro {#fluxo-registro}

```
┌─────────────────────────────────────────────────┐
│ 1. PÁGINA DE REGISTRO                           │
│    - Name, Email, Password, Password Confirm    │
│    - Validações em tempo real                   │
└──────────────────┬──────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────┐
│ 2. VALIDAÇÃO DE SENHA                           │
│    ✓ Mínimo 8 caracteres                        │
│    ✓ Pelo menos 1 maiúscula                     │
│    ✓ Pelo menos 1 minúscula                     │
│    ✓ Pelo menos 1 número                        │
│    ✓ Pelo menos 1 caractere especial            │
└──────────────────┬──────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────┐
│ 3. SUBMIT FORM                                  │
│    - POST /auth/register                        │
│    - Enviar name, email, password, confirmation │
└──────────────────┬──────────────────────────────┘
                   │
         ┌─────────┴────────────┐
         │                      │
         ▼                      ▼
    ✅ SUCESSO              ❌ ERRO
    (200 OK)               (400/409)
         │                      │
         ▼                      ▼
┌──────────────────┐    ┌──────────────────────┐
│ AUTO LOGIN       │    │ MOSTRAR ERROR        │
│ - Tokens criados │    │ - Email já existe    │
│ - Usuário criado │    │ - Dados inválidos    │
└────────┬─────────┘    │ - Tentar novamente   │
         │              └──────────────────────┘
         ▼
┌─────────────────────────────────────────────────┐
│ REDIRECT TO DASHBOARD                           │
└─────────────────────────────────────────────────┘
```

---

## 📊 Dashboard com Dados do Usuário {#dashboard}

### Exemplo de Atualização em Real-Time

```typescript
// src/components/UserGreeting.tsx

import React from 'react';
import { useAuth } from '../hooks/useAuth';

export const UserGreeting: React.FC = () => {
  const { user } = useAuth();

  const getGreeting = () => {
    const hour = new Date().getHours();
    if (hour < 12) return 'Bom dia';
    if (hour < 18) return 'Boa tarde';
    return 'Boa noite';
  };

  return (
    <div className="greeting">
      <h1>{getGreeting()}, {user?.name}! 👋</h1>
      <p className="subtitle">
        Bem-vindo ao ZetaFin
      </p>
    </div>
  );
};
```

### Componente de Profile

```typescript
// src/components/UserProfile.tsx

import React from 'react';
import { useAuth } from '../hooks/useAuth';

export const UserProfile: React.FC = () => {
  const { user } = useAuth();

  if (!user) return null;

  return (
    <div className="profile-card">
      <div className="profile-header">
        <div className="avatar">
          {user.name.charAt(0).toUpperCase()}
        </div>
        <div className="profile-info">
          <h3>{user.name}</h3>
          <p>{user.email}</p>
          <span className="role">{user.role}</span>
        </div>
      </div>

      <div className="profile-stats">
        <div className="stat">
          <label>Membro desde</label>
          <value>{new Date(user.createdAt).toLocaleDateString('pt-BR')}</value>
        </div>
        {user.lastLoginAt && (
          <div className="stat">
            <label>Último login</label>
            <value>{new Date(user.lastLoginAt).toLocaleString('pt-BR')}</value>
          </div>
        )}
      </div>
    </div>
  );
};
```

---

## ⚠️ Tratamento de Erros {#tratamento-erros}

### Error Boundary
```typescript
// src/components/ErrorBoundary.tsx

import React from 'react';

interface Props {
  children: React.ReactNode;
}

interface State {
  hasError: boolean;
  error?: Error;
}

export class ErrorBoundary extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    console.error('ErrorBoundary caught:', error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="error-container">
          <h2>Algo deu errado</h2>
          <p>{this.state.error?.message}</p>
          <button onClick={() => window.location.reload()}>
            Recarregar página
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}
```

### Error Handling in Components
```typescript
// Exemplo de tratamento de erro completo

const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  clearError();

  try {
    setIsLoading(true);

    // Validação
    if (!validateForm()) {
      return;
    }

    // API Call
    await login(credentials);

    // Sucesso
    navigate('/dashboard');
  } catch (err: any) {
    // O erro já está no context.error
    // Mas podemos adicionar tratamento específico

    if (err.response?.status === 401) {
      setError('Email ou senha inválidos');
    } else if (err.response?.status === 429) {
      setError('Muitas tentativas. Tente novamente em 30 minutos');
    } else if (err.response?.status === 500) {
      setError('Erro no servidor. Tente novamente mais tarde');
    } else {
      setError(err.message || 'Erro ao fazer login');
    }
  } finally {
    setIsLoading(false);
  }
};
```

---

## 📱 Mobile (React Native) - Diferenças {#mobile-react-native}

### Setup para React Native
```typescript
// src/services/storage/secureStorage.ts

import * as SecureStore from 'expo-secure-store';

export const SecureStorage = {
  async setItem(key: string, value: string): Promise<void> {
    try {
      await SecureStore.setItemAsync(key, value);
    } catch (error) {
      console.error('Error storing secure item:', error);
    }
  },

  async getItem(key: string): Promise<string | null> {
    try {
      return await SecureStore.getItemAsync(key);
    } catch (error) {
      console.error('Error retrieving secure item:', error);
      return null;
    }
  },

  async removeItem(key: string): Promise<void> {
    try {
      await SecureStore.deleteItemAsync(key);
    } catch (error) {
      console.error('Error removing secure item:', error);
    }
  },
};

// Uso em Mobile:
// const token = await SecureStorage.getItem('accessToken');
// await SecureStorage.setItem('accessToken', token);
// await SecureStorage.removeItem('accessToken');
```

### AuthService para Mobile
```typescript
// src/services/api/authServiceMobile.ts (React Native)

export class AuthServiceMobile {
  static async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await apiClient.post('/authentication/login', {
      ...credentials,
      deviceName: await this.getDeviceName(),
      deviceType: 'Mobile', // ou 'iOS' / 'Android'
    });

    // Armazenar tokens em Secure Storage
    await SecureStorage.setItem('accessToken', response.accessToken);
    await SecureStorage.setItem('refreshToken', response.refreshToken);

    return response;
  }

  private static async getDeviceName(): Promise<string> {
    // Usar react-native-device-info
    const deviceName = await DeviceInfo.getDeviceName();
    return deviceName;
  }
}
```

### API Client com Biometria (Mobile)
```typescript
// src/services/api/apiClientMobile.ts

import * as LocalAuthentication from 'expo-local-authentication';

export class ApiClientMobile {
  static async authenticateWithBiometry(): Promise<boolean> {
    try {
      const compatible = await LocalAuthentication.hasHardwareAsync();
      if (!compatible) return false;

      const enrolled = await LocalAuthentication.isEnrolledAsync();
      if (!enrolled) return false;

      const authenticated = await LocalAuthentication.authenticateAsync({
        disableDeviceFallback: false,
        reason: 'Autentique-se para acessar ZetaFin',
      });

      return authenticated.success;
    } catch (error) {
      console.error('Biometric auth error:', error);
      return false;
    }
  }
}
```

---

## 🔐 Segurança {#segurança}

### Web - Boas Práticas

```typescript
// ✅ FAZER

// 1. Usar HTTPS em produção
const API_BASE_URL = 'https://api.zetafin.com/api';

// 2. Armazenar tokens em localStorage (Web)
// localStorage é acessível apenas pelo seu domínio

// 3. Adicionar token no Authorization Header
axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

// 4. Renovar token automaticamente
// Implementado no interceptor

// 5. Fazer logout ao detectar token inválido
if (error.status === 401) {
  localStorage.removeItem('accessToken');
  navigate('/login');
}

// 6. Validar dados antes de enviar
if (!validateForm()) return;

// 7. Usar HttpOnly cookies para refresh token (via backend)
// Verificar com backend se pode usar HttpOnly

// 8. Limpar dados ao logout
localStorage.clear();
sessionStorage.clear();

// ❌ NÃO FAZER

// 1. Armazenar tokens em localStorage em apps muito sensíveis
// (em Electron, usar safeStorage)

// 2. Expor tokens em logs
console.log(token); // ❌

// 3. Enviar tokens por URL
`/api/endpoint?token=${token}` // ❌

// 4. Armazenar múltiplos tokens sem encriptação
// Usar estrutura de dados segura

// 5. Confiar apenas em validação frontend
// Sempre validar no backend também

// 6. Usar senhas fracas
// Implementar requisitos forte (já feito no backend)

// 7. Armazenar dados sensíveis em sessionStorage
// Usar localStorage com SameSite cookies
```

### Mobile - Boas Práticas

```typescript
// ✅ FAZER - React Native

// 1. Usar Secure Store (Keychain/Keystore)
import * as SecureStore from 'expo-secure-store';
await SecureStore.setItemAsync('token', token);

// 2. Usar Biometria para revalidação
LocalAuthentication.authenticateAsync();

// 3. Implementar timeout de inatividade
const timeout = setTimeout(() => logout(), 15 * 60 * 1000); // 15 min

// 4. Usar HTTPS apenas
// Configurar SSL pinning para API calls

// 5. Limpar tokens ao logout
await SecureStore.deleteItemAsync('accessToken');

// 6. Criptografar dados sensíveis no device
// Usar expo-crypto

// 7. Respeitar biometric auth do device
// Verificar se habilitada antes de usar

// ❌ NÃO FAZER - React Native

// 1. Armazenar tokens em AsyncStorage (plain text)
AsyncStorage.setItem('token', token); // ❌

// 2. Exibir tokens em logs/console
console.log(token); // ❌

// 3. Transmitir tokens por HTTP
// Sempre usar HTTPS

// 4. Reutilizar mesma sessão entre usuários
// Fazer logout completo ao trocar user

// 5. Armazenar senhas no device
// Nunca armazenar senha, apenas tokens
```

---

## 🚀 Setup do Projeto React

### 1. Instalar Dependências
```bash
npm install axios react-router-dom react

# TypeScript
npm install --save-dev typescript @types/react @types/react-dom @types/node

# Environment
npm install dotenv
```

### 2. Arquivo .env
```env
REACT_APP_API_URL=http://localhost:5001/api
REACT_APP_ENVIRONMENT=development
```

### 3. App.tsx com Routing
```typescript
// src/App.tsx

import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Dashboard } from './pages/Dashboard';
import { ErrorBoundary } from './components/ErrorBoundary';
import './App.css';

function App() {
  return (
    <ErrorBoundary>
      <Router>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </AuthProvider>
      </Router>
    </ErrorBoundary>
  );
}

export default App;
```

---

## 📋 Checklist de Implementação

- [ ] Instalar dependências (axios, react-router-dom)
- [ ] Criar estrutura de tipos (auth.ts, api.ts)
- [ ] Implementar ApiClient com interceptors
- [ ] Criar AuthService
- [ ] Criar AuthContext
- [ ] Criar useAuth hook
- [ ] Criar Login component
- [ ] Criar Register component
- [ ] Criar Dashboard component
- [ ] Implementar roteamento
- [ ] Testar fluxo de login
- [ ] Testar fluxo de registro
- [ ] Testar logout
- [ ] Testar refresh token
- [ ] Testar sessões
- [ ] Implementar mobile (React Native) - futuro
- [ ] Adicionar testes unitários
- [ ] Setup CI/CD

---

## 🧪 Exemplo de Teste Unitário

```typescript
// src/__tests__/useAuth.test.tsx

import { renderHook, act } from '@testing-library/react';
import { useAuth } from '../hooks/useAuth';
import { AuthProvider } from '../contexts/AuthContext';

describe('useAuth', () => {
  it('should initialize with no user', () => {
    const { result } = renderHook(() => useAuth(), {
      wrapper: AuthProvider,
    });

    expect(result.current.user).toBeNull();
    expect(result.current.isAuthenticated).toBe(false);
  });

  it('should login successfully', async () => {
    const { result } = renderHook(() => useAuth(), {
      wrapper: AuthProvider,
    });

    await act(async () => {
      await result.current.login({
        email: 'test@example.com',
        password: 'TestPass123!',
      });
    });

    expect(result.current.isAuthenticated).toBe(true);
    expect(result.current.user).toBeDefined();
  });
});
```

---

## 📞 Próximos Passos

1. **Hoje**: Setup inicial do projeto React
2. **Semana 1**: Implementar autenticação básica (login/register)
3. **Semana 2**: Dashboard e gerenciamento de sessões
4. **Semana 3**: Testes e refinamentos
5. **Semana 4**: Mobile (React Native)

---

## 📚 Referências

- [React Documentation](https://react.dev)
- [React Router](https://reactrouter.com)
- [Axios](https://axios-http.com)
- [TypeScript](https://www.typescriptlang.org)
- [Expo Secure Store](https://docs.expo.dev/modules/expo-secure-store/)
- [React Native](https://reactnative.dev)
