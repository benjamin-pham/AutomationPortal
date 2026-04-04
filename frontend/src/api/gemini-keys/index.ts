import { AxiosInstance } from "axios";
import { getGeminiKeys } from "./getGeminiKeys";
import { createGeminiKey } from "./createGeminiKey";
import { updateGeminiKey } from "./updateGeminiKey";
import { deleteGeminiKey } from "./deleteGeminiKey";
import {
  CreateGeminiKeyRequest,
  UpdateGeminiKeyRequest,
} from "./types";

export * from "./types";

const geminiKeysApi = (axios: AxiosInstance) => ({
  getGeminiKeys: (pageNumber: number, pageSize: number) => getGeminiKeys(axios, pageNumber, pageSize),
  createGeminiKey: (request: CreateGeminiKeyRequest) => createGeminiKey(axios, request),
  updateGeminiKey: (id: string, request: UpdateGeminiKeyRequest) => updateGeminiKey(axios, id, request),
  deleteGeminiKey: (id: string) => deleteGeminiKey(axios, id),
});

export default geminiKeysApi;
