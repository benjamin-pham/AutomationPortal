import { AxiosInstance } from "axios";

export interface ProfileResponse {
  userId: string;
  firstName: string;
  lastName: string;
  username: string;
  email?: string;
  phone?: string;
  birthday?: string;
  createdAt: string;
}

export const getProfile = (axios: AxiosInstance) => () => {
  return axios.get<ProfileResponse>("/api/auth/profile");
};
