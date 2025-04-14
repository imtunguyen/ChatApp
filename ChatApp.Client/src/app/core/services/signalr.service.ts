import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { Message } from '../models/message.module';
import { BehaviorSubject, Subject } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;
  public onlineUsers$ = new BehaviorSubject<string[]>([]);
  public newMessage$ = new Subject<Message | null>();
  public connectionEstablished$ = new BehaviorSubject<boolean>(false);

  private chatUrl = environment.chatUrl;

  constructor() { 
    
  }

  startConnection(userId: string) {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      console.error("⚠️ Không tìm thấy accessToken trong localStorage!");
      return;
    }
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      console.warn("⚠️ SignalR connection already started!");
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.chatUrl, {
        
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('✅ SignalR connection started');
        this.connectionEstablished$.next(true);
        this.getOnlineUsers();
      })
      .catch(err => {
        console.error('❌ Error starting SignalR:', err);
        this.connectionEstablished$.next(false); 
      });

      this.hubConnection.onreconnected(() => {

        this.getOnlineUsers();
      });
      
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
    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      console.log('📩 [DEBUG] Nhận tin nhắn từ SignalR:', message);
      if (message) {
        this.newMessage$.next(message);
      }
    });

    this.hubConnection.on('ReceiveNotification', (message: Message) => {
      
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
    if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('GetOnlineUsers')
        .catch(err => console.error(err));
    } else {
      console.warn("SignalR chưa sẵn sàng, bỏ qua getOnlineUsers");
    }
  }
  

  sendMessage(message: Message) {
    console.log("🔄 Kiểm tra trạng thái SignalR:", this.hubConnection.state);
  
    if (this.hubConnection.state !== signalR.HubConnectionState.Connected) {
      console.error("❌ SignalR chưa kết nối, không thể gửi tin nhắn.");
      return;
    }
    console.log("🔄 Gửi tin nhắn qua SignalR:", message);
    if(message.groupId == null) {
      this.hubConnection.invoke('SendPrivateMessage', message)
        .then(() => console.log("✅ Tin nhắn riêng đã được gửi", message))
        .catch(err => console.error("❌ Gửi tin nhắn riêng thất bại:", err));
    }
    else {
      this.hubConnection.invoke('SendGroupMessage', message)
        .then(() => console.log("✅ Tin nhắn nhóm đã được gửi", message))
        .catch(err => console.error("❌ Gửi tin nhắn nhóm thất bại:", err));
    }

  }
  

  joinGroup(groupId: number) {
    this.hubConnection.invoke('JoinGroup', groupId)
      .then(() => console.log(`📌 Đã tham gia nhóm ${groupId}`))
      .catch(err => console.error("❌ Lỗi tham gia nhóm:", err));
  }

  leaveGroup(groupId: string) {
    this.hubConnection.invoke('LeaveChatRoom', groupId)
      .then(() => console.log(`🚪 Rời nhóm ${groupId}`))
      .catch(err => console.error("❌ Lỗi rời nhóm:", err));
  }
  
  // Trong SignalRService
  async waitForConnection(): Promise<void> {
    return new Promise((resolve) => {
      if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
        resolve();
      } else {
        const check = setInterval(() => {
          if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
            clearInterval(check);
            resolve();
          }
        }, 100);
      }
    });
  }

  

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
