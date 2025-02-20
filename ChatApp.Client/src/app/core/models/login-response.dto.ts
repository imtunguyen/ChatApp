export interface LoginResponse {
    user: UserDto;
    accessToken: string;
    refreshToken: string;
    refreshTokenExpiredTime: Date;
  }
  
  export interface UserDto {
    id: string;
    fullName: string;
    userName: string;
    email: string;
    avatar: string;
  }