import axios, { AxiosRequestConfig } from 'axios';

// Create a globally configurable axios instance (add auth headers or interceptors here)
const axiosInstance = axios.create({
    baseURL: process.env.NEXT_PUBLIC_API_URL || '',
    headers: {
        'Content-Type': 'application/json',
    },
});

export const apiRequest = async <T>(
    url: string,
    method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH',
    body?: any,
    params?: any,
    options?: AxiosRequestConfig
): Promise<T> => {
    if (body && !(body instanceof FormData)) {
        options = {
            ...options,
            headers: {
                'Content-Type': 'application/json',
                ...(options?.headers || {}),
            },
        };
    }

    const response = await axiosInstance.request<T>({
        url,
        method,
        data: body,
        params,
        ...options,
    });
    return response.data;
};
