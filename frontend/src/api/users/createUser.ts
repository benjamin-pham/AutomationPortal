import { AxiosInstance } from "axios";

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  username: string;
  password: string;
  email?: string;
  phone?: string;
  birthday?: string;
}

export interface CreateUserResponse {
  id: string;
  username: string;
}

export const createUser = (axios: AxiosInstance, data: CreateUserRequest): Promise<CreateUserResponse> => {
  return axios.post<CreateUserResponse>("/api/users", data).then((res) => res.data);
};
