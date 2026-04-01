import { AxiosInstance } from "axios";
import { LoginRequest, login } from "@/api/auth/login";
import { RegisterRequest, register } from "@/api/auth/register";
import { RefreshTokenRequest, refreshToken } from "@/api/auth/refreshToken";
import { getProfile } from "@/api/auth/getProfile";
import { UpdateProfileRequest, updateProfile } from "@/api/auth/updateProfile";

const authApi = (axios: AxiosInstance) => ({
  login: (data: LoginRequest) => login(axios, data),
  register: (data: RegisterRequest) => register(axios, data),
  refreshToken: (data: RefreshTokenRequest) => refreshToken(axios, data),
  getProfile: () => getProfile(axios),
  updateProfile: (data: UpdateProfileRequest) => updateProfile(axios, data),
});

export default authApi;
