export interface Group {
  avatarUrl: string;
  id: number;
  name: string;
  creator: string;
  createdAt: Date;
  updatedAt: Date;
  lastMessage: string;
  lastMessageTime: Date;
  hasNewMessage : boolean;
}

export interface AddGroup {
  name: string;
  creator: string;
  users: string[];
}
