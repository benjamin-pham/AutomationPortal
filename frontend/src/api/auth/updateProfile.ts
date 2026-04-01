import { AxiosInstance } from "axios";

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  email?: string | null;
  phone?: string | null;
  birthday?: string | null;
}

export const updateProfile = (axios: AxiosInstance, data: UpdateProfileRequest) => () => {
  return axios.put<void>("/api/auth/profile", data);
};
