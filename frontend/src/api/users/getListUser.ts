import { AxiosInstance } from "axios";
import { GetUsersParams, PagedResponse, UserListItem } from "./types";

export const getListUser = (axios: AxiosInstance, params: GetUsersParams): Promise<PagedResponse<UserListItem>> => {
  return axios.get<PagedResponse<UserListItem>>("/api/users", { params }).then((res) => res.data);
};
