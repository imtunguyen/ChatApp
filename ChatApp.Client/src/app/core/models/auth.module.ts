import { Gender } from "./gender";

export interface Login
{
  userNameOrEmail: string;
  password: string;
}
export interface Register
{
  fullName: string;
  userName: string;
  email: string;
  password: string;
  gender: Gender;
  profilePicture: File;
}
