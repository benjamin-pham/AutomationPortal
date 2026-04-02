import { AxiosInstance } from "axios";
import { getListUser } from "./getListUser";
import { getUserById } from "./getUserById";
import { createUser } from "./createUser";
import { updateUser } from "./updateUser";
import { deleteUser } from "./deleteUser";
import { resetUserPassword } from "./resetUserPassword";
import {
  GetUsersParams,
  CreateUserRequest,
  UpdateUserRequest,
  ResetPasswordRequest,
} from "./types";

export * from "./types";

const usersApi = (axios: AxiosInstance) => ({
  getListUser: (params: GetUsersParams) => getListUser(axios, params),
  getUserById: (id: string) => getUserById(axios, id),
  createUser: (data: CreateUserRequest) => createUser(axios, data),
  updateUser: (id: string, data: UpdateUserRequest) => updateUser(axios, id, data),
  deleteUser: (id: string) => deleteUser(axios, id),
  resetUserPassword: (id: string, data: ResetPasswordRequest) => resetUserPassword(axios, id, data),
});

export default usersApi;
