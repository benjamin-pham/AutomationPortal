import { AxiosInstance } from "axios";
import { TokenResponse } from "@/api/auth/login";

export interface RefreshTokenRequest {
  refreshToken: string;
}

export const refreshToken = (axios: AxiosInstance, data: RefreshTokenRequest) => () => {
  return axios.post<TokenResponse>("/api/auth/refresh-token", data);
};
