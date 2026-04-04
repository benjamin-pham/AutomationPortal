import { AxiosInstance } from "axios";

export const deleteGeminiKey = async (
  axios: AxiosInstance,
  id: string
): Promise<void> => {
  await axios.delete(`/api/gemini-keys/${id}`);
};
