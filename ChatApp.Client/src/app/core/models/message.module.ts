import { MessageType } from "./enum/message-type";

export interface MessageFile{
  id: number;
  url: string;
  fileType: number;
  fileName: string;
  fileSize: number;
}

export interface Message{
  id: number;
  content: string;
  senderId: string;
  recepientId?: string;
  groupId?: number;
  sentAt: Date;
  updatedAt: Date;
  status: string;
  isDeleted: boolean;
  type: MessageType;
  files: MessageFile[];
  isRead: boolean;
}
