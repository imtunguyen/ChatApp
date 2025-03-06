import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../shared/services/api.service';
import { Observable, Subject } from 'rxjs';
import io from 'socket.io-client';
@Injectable({
  providedIn: 'root'
})
export class UserStatusService {

  private apiService = inject(ApiService);
  private socket!: ReturnType<typeof io>;
  private userStatusSubject = new Subject<{ userId: string, isOnline: boolean }>();
  constructor() { }

  setUserOnline(userId: string) : Observable<void>{
    return this.apiService.post<void>(`userstatus/set-online/${userId}`, {});
  }

  setUserOffline(userId: string) : Observable<void> {
    return this.apiService.post<void>(`userstatus/set-offline/${userId}`, {});
  }

  isUserOnline(userId: string) : Observable<boolean> {
    return this.apiService.get<boolean>(`userstatus/is-online/${userId}`);
  }

  getOnlineUsersCount(): Observable<number> {
    return this.apiService.get<number>(`userstatus/online-users-count`);
  }

  getUsersOnlineStatus(userIds: string[]): Observable<{ [key: string]: boolean }> {
    return this.apiService.post<{ [key: string]: boolean }>(`userstatus/users-online-status`, userIds);
  }

  userStatusChanged(): Observable<{ userId: string, isOnline: boolean }> {
    return this.userStatusSubject.asObservable();
  }
  
}
