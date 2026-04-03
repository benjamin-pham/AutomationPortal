import { AxiosInstance } from "axios";
import { UpdateGeminiKeyRequest } from "./types";

export const updateGeminiKey = async (
  axios: AxiosInstance,
  id: string,
  request: UpdateGeminiKeyRequest
): Promise<void> => {
  await axios.put(`/api/gemini-keys/${id}`, request);
};
