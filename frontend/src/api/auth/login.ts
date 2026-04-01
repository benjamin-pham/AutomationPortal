import { AxiosInstance } from "axios";

export interface LoginRequest {
  username: string;
  password: string;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
}

export const login = (axios: AxiosInstance, data: LoginRequest) => () => {
  return axios.post<TokenResponse>("/api/auth/login", data);
};
