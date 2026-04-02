export interface UserListItem {
  id: string;
  firstName: string;
  lastName: string;
  username: string;
  email?: string;
  phone?: string;
  birthday?: string; // ISO date string "YYYY-MM-DD"
}

export interface UserDetail {
  id: string;
  firstName: string;
  lastName: string;
  username: string;
  email?: string;
  phone?: string;
  birthday?: string; // ISO date string "YYYY-MM-DD"
}

export interface PagedResponse<T> {
  items: T[];
  totalItems: number;
  totalPages: number;
  page: number;
  pageSize: number;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  username: string;
  password: string;
  email?: string;
  phone?: string;
  birthday?: string;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  birthday?: string;
}

export interface CreateUserResponse {
  id: string;
  username: string;
}

export interface ResetPasswordRequest {
  newPassword: string;
  confirmPassword: string;
}

export interface GetUsersParams {
  search?: string;
  sortColumn?: string; // snake_case: "first_name" | "last_name" | "username" | "email" | "phone" | "birthday"
  sortDirection?: "asc" | "desc";
  page?: number;
  pageSize?: number;
}
