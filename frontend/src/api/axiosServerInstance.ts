import axios, { AxiosInstance } from 'axios';
import { cookies } from 'next/headers';

const axiosServerInstance = async (): Promise<AxiosInstance> => {
  const instance = axios.create({
    baseURL: process.env.API_URL ?? 'http://172.26.0.14:5000',
  });

  // 👉 attach token từ cookie của Next server
  instance.interceptors.request.use(async (config) => {
    const cookieStore = await cookies();
    const token = cookieStore.get('accessToken')?.value;

    if (token) {
      config.headers = config.headers || {};
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  });

  // 👉 xử lý lỗi
  instance.interceptors.response.use(
    (res) => res,
    (error) => {
      console.log(error)
      return Promise.reject(error);
    }
  );

  return instance;
};

export default axiosServerInstance;