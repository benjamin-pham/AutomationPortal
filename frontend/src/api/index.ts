import authApi from "@/api/auth";
import usersApi from "@/api/users";
import geminiKeysApi from "@/api/gemini-keys";
import { AxiosInstance } from "axios";

const mainApi = (axios: AxiosInstance) => ({
  auth: authApi(axios),
  users: usersApi(axios),
  geminiKeys: geminiKeysApi(axios),
});
export default mainApi;