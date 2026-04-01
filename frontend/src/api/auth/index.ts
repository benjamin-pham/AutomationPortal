import { getProfile } from "@/api/auth/getProfile";
import { LoginRequest, login } from "@/api/auth/login";
import { AxiosInstance } from "axios";

const authApi = (axios: AxiosInstance) => ({
  login: (data: LoginRequest) => login(axios, data),
  getProfile: () => getProfile(axios),
});

export default authApi;
