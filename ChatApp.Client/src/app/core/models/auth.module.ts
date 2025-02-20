import { GENDER_LIST } from "./enum/gender";


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
  gender: keyof typeof GENDER_LIST;
  profilePicture: File;
}
