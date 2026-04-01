import { AxiosInstance } from "axios";

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  id: string;
  username: string;
}
export const login = (axios: AxiosInstance, data: LoginRequest) => () => {
  return axios.post<LoginResponse>("/auth/login", data);
};