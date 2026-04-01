import authApi from "@/api/auth";
import { AxiosInstance } from "axios";


const domainApi = (axios: AxiosInstance) => ({
  auth: authApi(axios)
});
export default domainApi;