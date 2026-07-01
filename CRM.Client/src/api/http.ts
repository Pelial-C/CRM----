import axios from 'axios'
import type { ApiResponse } from '../types/common'

export const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000/api',
  timeout: 15000,
  withCredentials: true,
})

async function unwrap<T>(request: Promise<{ data: ApiResponse<T> }>): Promise<T> {
  try {
    const response = await request
    if (!response.data.success) {
      throw new Error(response.data.message ?? '操作失败')
    }

    return response.data.data as T
  } catch (error) {
    if (axios.isAxiosError<ApiResponse<unknown>>(error)) {
      if (error.response?.status === 401 || error.response?.status === 403) {
        throw new Error('当前用户无权访问该资源')
      }

      const message = error.response?.data?.message
      throw new Error(message ?? '网络异常，请检查后端服务是否启动')
    }

    throw error
  }
}

export function apiGet<T>(url: string, params?: unknown) {
  return unwrap<T>(http.get<ApiResponse<T>>(url, { params }))
}

export function apiPost<T>(url: string, data?: unknown) {
  return unwrap<T>(http.post<ApiResponse<T>>(url, data))
}

export function apiPut<T>(url: string, data?: unknown) {
  return unwrap<T>(http.put<ApiResponse<T>>(url, data))
}

export function apiDelete<T>(url: string) {
  return unwrap<T>(http.delete<ApiResponse<T>>(url))
}
