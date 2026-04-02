import { AxiosInstance } from "axios";
import { UserDetail } from "./types";

export const getUserById = (axios: AxiosInstance, id: string): Promise<UserDetail> => {
  return axios.get<UserDetail>(`/api/users/${id}`).then((res) => res.data);
};
