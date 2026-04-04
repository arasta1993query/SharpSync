import axios, { AxiosRequestConfig } from 'axios';

// Create a globally configurable axios instance (add auth headers or interceptors here)
const axiosInstance = axios.create({
    baseURL: process.env.NEXT_PUBLIC_API_URL || '',
    headers: {
        'Content-Type': 'application/json',
    },
});

export const serializeToFormData = (obj: any): FormData => {
    const formData = new FormData();
    if (obj) {
        Object.entries(obj).forEach(([key, value]) => {
            if (value !== undefined && value !== null) {
                if (Array.isArray(value)) {
                    value.forEach(v => formData.append(key, v));
                } else {
                    formData.append(key, value as any);
                }
            }
        });
    }
    return formData;
};

export const serializeToQueryParams = (obj: any, prefix: string = ''): URLSearchParams => {
    const params = new URLSearchParams();
    const append = (key: string, value: any) => {
        const fullKey = prefix ? `${prefix}.${key}` : key;
        if (value === null || value === undefined) return;
        if (Array.isArray(value)) {
            value.forEach(v => {
                if (v !== null && v !== undefined) params.append(fullKey, v.toString());
            });
        } else if (typeof value === 'object' && !(value instanceof Date)) {
            const innerParams = serializeToQueryParams(value, fullKey);
            innerParams.forEach((v, k) => params.append(k, v));
        } else {
            params.append(fullKey, value instanceof Date ? value.toISOString() : value.toString());
        }
    };
    if (obj && typeof obj === 'object') {
        Object.entries(obj).forEach(([key, value]) => append(key, value));
    }
    return params;
};

export interface ApiErrorResponse {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    errors?: Record<string, string[]>;
}

export class SharpSyncError extends Error {
    constructor(public response: ApiErrorResponse, public statusCode: number) {
        super(response.title || response.detail || 'An API error occurred');
        this.name = 'SharpSyncError';
    }
}

export const apiRequest = async <T>(
    url: string,
    method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH',
    body?: any,
    params?: any,
    options?: AxiosRequestConfig
): Promise<T> => {
    const queryParams = params ? serializeToQueryParams(params) : undefined;

    if (body && !(body instanceof FormData)) {
        options = {
            ...options,
            headers: {
                'Content-Type': 'application/json',
                ...(options?.headers || {}),
            },
        };
    }

    try {
        const response = await axiosInstance.request<T>({
            url,
            method,
            data: body,
            params: queryParams,
            ...options,
        });
        return response.data;
    } catch (error: any) {
        if (error.response) {
            throw new SharpSyncError(error.response.data as ApiErrorResponse, error.response.status);
        }
        throw error;
    }
};
