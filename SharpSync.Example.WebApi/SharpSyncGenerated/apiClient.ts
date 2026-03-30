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
    options?: AxiosRequestConfig
): Promise<T> => {
    const response = await axiosInstance.request<T>({
        url,
        method,
        data: body,
        ...options,
    });
    return response.data;
};
