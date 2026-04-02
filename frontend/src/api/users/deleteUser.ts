import type { AxiosInstance } from "axios";

export async function deleteUser(axios: AxiosInstance, id: string): Promise<void> {
  await axios.delete(`/api/users/${id}`);
}
