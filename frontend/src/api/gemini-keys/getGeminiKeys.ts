import { AxiosInstance } from "axios";
import { GeminiKeyPagedResponse } from "./types";

export const getGeminiKeys = (
  axios: AxiosInstance,
  pageNumber: number,
  pageSize: number
): Promise<GeminiKeyPagedResponse> => {
  return axios
    .get<GeminiKeyPagedResponse>("/api/gemini-keys", {
      params: { pageNumber, pageSize },
    })
    .then((res) => res.data);
};
