import { User } from "./user.model";

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: User;
}
