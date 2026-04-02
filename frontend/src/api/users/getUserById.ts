import { AxiosInstance } from "axios";

export interface UserDetail {
  id: string;
  firstName: string;
  lastName: string;
  username: string;
  email?: string;
  phone?: string;
  birthday?: string; // ISO date string "YYYY-MM-DD"
}

export const getUserById = (axios: AxiosInstance, id: string): Promise<UserDetail> => {
  return axios.get<UserDetail>(`/api/users/${id}`).then((res) => res.data);
};
