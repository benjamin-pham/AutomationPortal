import type { AxiosInstance } from "axios";

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  birthday?: string;
}

export async function updateUser(
  axios: AxiosInstance,
  id: string,
  data: UpdateUserRequest
): Promise<void> {
  await axios.put(`/api/users/${id}`, data);
}
