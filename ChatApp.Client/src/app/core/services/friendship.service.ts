import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../shared/services/api.service";
import { FriendShip, updateFriendShip } from "../models/friendship.module";
import { FriendShipStatus } from "../models/enum/friendship-status";
import { BehaviorSubject } from "rxjs";


@Injectable({
  providedIn: 'root'
})
export class FriendShipService {

  private api = inject(ApiService);

  private friendShipUpdateSubject = new BehaviorSubject<void>(undefined);
    friendShipUpdate = this.friendShipUpdateSubject.asObservable();
    constructor() { }
  addFriendShip(requesterId: string, addresseeId: string) {
    const friendship = {
      requesterId,
      addresseeId
    };
    return this.api.post<FriendShip>('friendship/add', friendship);
  }

  updateFriendShip(requesterId: string, addresseeId: string, status: FriendShipStatus) {
    const friendship= {
      requesterId,
      addresseeId,
      status
    };
    console.log(friendship);
    return this.api.put('friendship/update', friendship);
  }
  getFriendShips(requesterId: string, addresseeId: string){
    return this.api.get<FriendShip>(`friendship/get?requesterId=${requesterId}&addresseeId=${addresseeId}`);
  }

  getFriendShipsByUser(userId: string, status: number){
    return this.api.get<FriendShip[]>(`friendship/GetByUserId?userId=${userId}&status=${status}`);
  }

  getPendingRequests(userId: string){
    return this.api.get<FriendShip[]>(`friendship/pending-requests/${userId}`);
  }

  getFriends(userId: string){
    return this.api.get<FriendShip[]>(`friendship/friends/${userId}`);
  }

  notifyFriendShipUpdate(){
    this.friendShipUpdateSubject.next();
  }
}
