import { AxiosInstance } from "axios";
import { CreateGeminiKeyRequest, CreateGeminiKeyResponse } from "./types";

export const createGeminiKey = async (
  axios: AxiosInstance,
  request: CreateGeminiKeyRequest
): Promise<CreateGeminiKeyResponse> => {
  const response = await axios.post<CreateGeminiKeyResponse>(
    "/api/gemini-keys",
    request
  );
  return response.data;
};
