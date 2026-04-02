import type { AxiosInstance } from "axios";

export interface ResetPasswordRequest {
  newPassword: string;
  confirmPassword: string;
}

export async function resetUserPassword(
  axios: AxiosInstance,
  id: string,
  data: ResetPasswordRequest
): Promise<void> {
  await axios.post(`/api/users/${id}/reset-password`, data);
}
