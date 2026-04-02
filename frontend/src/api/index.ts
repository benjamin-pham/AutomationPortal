import authApi from "@/api/auth";
import usersApi from "@/api/users";
import { AxiosInstance } from "axios";

const domainApi = (axios: AxiosInstance) => ({
  auth: authApi(axios),
  users: usersApi(axios),
});
export default domainApi;