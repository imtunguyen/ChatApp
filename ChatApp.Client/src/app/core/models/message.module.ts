import { MessageType } from "./enum/message-type";

export interface File{
  id: number;
  url: string;
}

export interface Message{
  id: number;
  content: string;
  senderId: string;
  recepientId?: string;
  chatRoomId?: number;
  sentAt: Date;
  updatedAt: Date;
  status: string;
  type: MessageType;
  files: File[];
}
