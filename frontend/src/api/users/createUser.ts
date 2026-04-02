import { AxiosInstance } from "axios";
import { CreateUserRequest, CreateUserResponse } from "./types";

export const createUser = (axios: AxiosInstance, data: CreateUserRequest): Promise<CreateUserResponse> => {
  return axios.post<CreateUserResponse>("/api/users", data).then((res) => res.data);
};
