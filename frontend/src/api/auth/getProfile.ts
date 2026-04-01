import { AxiosInstance } from "axios";


export interface ProfileResponse {
  id: string;
  username: string;
}

export const getProfile = (axios: AxiosInstance) => () => {
  return axios.post<ProfileResponse>("/auth/profile");
};