import { AxiosInstance } from "axios";
export interface UpdateProfileRequest {
  id: string;
  username: string;
}

export interface UpdateProfileResponse {
  id: string;
  username: string;
}

export const updateProfile = (axios: AxiosInstance) => () => {
  return axios.put<UpdateProfileResponse>("/auth/profile");
};