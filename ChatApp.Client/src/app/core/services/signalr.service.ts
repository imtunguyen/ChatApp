import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { Message } from '../models/message.module';
import { BehaviorSubject } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;
  public onlineUsers$ = new BehaviorSubject<string[]>([]);
  public newMessage$ = new BehaviorSubject<string | null>(null);
  private chatUrl = environment.chatUrl;

  constructor() { 
    
  }

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.chatUrl, {
        accessTokenFactory: () => {
          const token = localStorage.getItem('accessToken');
          return token || '';
        }
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started');
        this.getOnlineUsers();
      })
      .catch(err => console.error('Error connecting SignalR:', err));
    
    this.hubConnection.on('UserOnline', (userId: string) => {
      console.log("UserOnline", userId)
      this.updateOnlineUsers(userId, true);
    });

    this.hubConnection.on('UserOffline', (userId: string) => {
      this.updateOnlineUsers(userId, false);
    });

    this.hubConnection.on('ReceiveOnlineUsers', (users: string[]) => {
      if (JSON.stringify(users) !== JSON.stringify(this.onlineUsers$.getValue())) {
        this.onlineUsers$.next(users);
      }
    });
    

    this.hubConnection.on('NewMessageNotification', (userId: string) => {
      this.newMessage$.next(userId);
    });
  }

  private updateOnlineUsers(userId: string, isOnline: boolean) {
    const users = this.onlineUsers$.getValue();
    if (isOnline && !users.includes(userId)) {
      this.onlineUsers$.next([...users, userId]);
    } else if (!isOnline) {
      this.onlineUsers$.next(users.filter(id => id !== userId));
    }
  }

  getOnlineUsers() {
    setTimeout(() => {
      if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
        this.hubConnection.invoke('GetOnlineUsers')
          .catch(err => console.error(err));
      } else {
        console.warn("SignalR connection is not established yet, retrying in 2 seconds...");
      }
    }, 2000); 
  }
  

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
