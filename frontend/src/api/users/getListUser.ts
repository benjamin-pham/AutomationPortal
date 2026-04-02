import { AxiosInstance } from "axios";
import { PagedList } from "@/types/PagedList";

export interface UserListItem {
  id: string;
  firstName: string;
  lastName: string;
  username: string;
  email?: string;
  phone?: string;
  birthday?: string; // ISO date string "YYYY-MM-DD"
}

export interface GetUsersParams {
  search?: string;
  sortColumn?: string; // snake_case: "first_name" | "last_name" | "username" | "email" | "phone" | "birthday"
  sortDirection?: "asc" | "desc";
  page?: number;
  pageSize?: number;
}

export const getListUser = (axios: AxiosInstance, params: GetUsersParams): Promise<PagedList<UserListItem>> => {
  return axios.get<PagedList<UserListItem>>("/api/users", { params }).then((res) => res.data);
};
