import { AxiosInstance } from "axios";

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  username: string;
  password: string;
  email?: string;
  phone?: string;
  birthday?: string;
}

export interface RegisterResponse {
  userId: string;
  username: string;
  firstName: string;
  lastName: string;
}

export const register = (axios: AxiosInstance, data: RegisterRequest) => () => {
  return axios.post<RegisterResponse>("/api/auth/register", data);
};
