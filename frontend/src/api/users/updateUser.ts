import type { AxiosInstance } from "axios";
import type { UpdateUserRequest } from "./types";

export async function updateUser(
  axios: AxiosInstance,
  id: string,
  data: UpdateUserRequest
): Promise<void> {
  await axios.put(`/api/users/${id}`, data);
}
