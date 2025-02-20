import { FriendShipStatus } from "./enum/friendship-status";

export interface FriendShip {
  id: number;
  requesterId: string;
  addresseeId: string;
  createdAt: Date;
  acceptedAt: Date;
  status: FriendShipStatus;
}

export interface updateFriendShip {
  requesterId: string;
  addresseeId: string;
  status: FriendShipStatus;
}
