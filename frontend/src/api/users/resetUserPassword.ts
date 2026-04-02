import type { AxiosInstance } from "axios";
import type { ResetPasswordRequest } from "./types";

export async function resetUserPassword(
  axios: AxiosInstance,
  id: string,
  data: ResetPasswordRequest
): Promise<void> {
  await axios.post(`/api/users/${id}/reset-password`, data);
}
