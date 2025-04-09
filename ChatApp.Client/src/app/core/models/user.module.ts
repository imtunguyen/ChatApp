export interface User {
  id: string;
  fullName: string;
  userName: string;
  email: string
  profilePictureUrl: string;
  token: string;
  gender: string;
  isOnline: boolean;
  isSelected: boolean;
  roleId: string;
  roleName: string;
  lastMessage: string;
  lastMessageTime: Date;
  isMember: boolean;
  hasNewMessage : boolean;
}
