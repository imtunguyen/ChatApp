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
        this.hubConnection.invoke('Connect', userId)
        this.getOnlineUsers();
      })
      .catch(err => console.error('❌ Error starting SignalR:', err));
    
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
    setTimeout(() => {
      if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
        this.hubConnection.invoke('GetOnlineUsers')
          .catch(err => console.error(err));
      } else {
        console.warn("SignalR connection is not established yet, retrying in 2 seconds...");
      }
    }, 2000); 
  }

  sendMessage(message: Message) {
    console.log("🔄 Kiểm tra trạng thái SignalR:", this.hubConnection.state);
  
    if (this.hubConnection.state !== signalR.HubConnectionState.Connected) {
      console.error("❌ SignalR chưa kết nối, không thể gửi tin nhắn.");
      return;
    }

    if(message.groupId == null) {
      this.hubConnection.invoke('SendPrivateMessage', message)
        .then(() => console.log("✅ Tin nhắn riêng đã được gửi", message))
        .catch(err => console.error("❌ Gửi tin nhắn riêng thất bại:", err));
    }
  
    // if (userId) {
    //   console.log("📩 Gửi tin nhắn riêng đến", userId);
    //   this.hubConnection.invoke('SendPrivateMessage', userId, message)
    //     .then(() => console.log("✅ Tin nhắn riêng đã được gửi"))
    //     .catch(err => console.error("❌ Gửi tin nhắn riêng thất bại:", err));
    // } else if (groupId) {
    //   console.log("📩 Gửi tin nhắn nhóm đến", groupId);
    //   this.hubConnection.invoke('SendGroupMessage', groupId, message)
    //     .then(() => console.log("✅ Tin nhắn nhóm đã được gửi"))
    //     .catch(err => console.error("❌ Gửi tin nhắn nhóm thất bại:", err));
    // } else {
    //   console.error("❌ Cần cung cấp userId hoặc groupId để gửi tin nhắn.");
    // }
  }
  

  joinGroup(groupId: string) {
    this.hubConnection.invoke('JoinChatRoom', groupId)
      .then(() => console.log(`📌 Đã tham gia nhóm ${groupId}`))
      .catch(err => console.error("❌ Lỗi tham gia nhóm:", err));
  }

  leaveGroup(groupId: string) {
    this.hubConnection.invoke('LeaveChatRoom', groupId)
      .then(() => console.log(`🚪 Rời nhóm ${groupId}`))
      .catch(err => console.error("❌ Lỗi rời nhóm:", err));
  }
  
  

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
