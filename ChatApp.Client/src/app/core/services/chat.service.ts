import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;
  private chatUrl = environment.chatUrl;
  public messages = signal<string[]>([]);

  constructor() { 
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.chatUrl, {
        withCredentials: false,
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.startConnection();
  }

  private startConnection() {
    this.hubConnection.start().then(() => {
      console.log("✅ Kết nối SignalR thành công!");
    }).catch(err => console.error("❌ Kết nối thất bại:", err));

    this.hubConnection.on('ReceiveMessage', (message: string) => {
      this.messages.update((currentMessages) => [...currentMessages, message]);
      console.log("📩 Nhận tin nhắn:", message);
    });
  }

  public sendMessage(message: string, userId?: string, groupId?: string) {
    if (this.hubConnection.state !== signalR.HubConnectionState.Connected) {
      console.error("❌ Kết nối SignalR chưa sẵn sàng!");
      return;
    }

    if (userId) {
      this.hubConnection.invoke('SendPrivateMessage', userId, message)
        .catch(err => console.error("❌ Gửi tin nhắn riêng thất bại:", err));
    } else if (groupId) {
      this.hubConnection.invoke('SendGroupMessage', groupId, message)
        .catch(err => console.error("❌ Gửi tin nhắn nhóm thất bại:", err));
    } else {
      console.error("❌ Cần cung cấp userId hoặc groupId để gửi tin nhắn.");
    }
  }

  public joinGroup(groupId: string) {
    this.hubConnection.invoke('JoinChatRoom', groupId)
      .then(() => console.log(`📌 Đã tham gia nhóm ${groupId}`))
      .catch(err => console.error("❌ Lỗi tham gia nhóm:", err));
  }

  public leaveGroup(groupId: string) {
    this.hubConnection.invoke('LeaveChatRoom', groupId)
      .then(() => console.log(`🚪 Rời nhóm ${groupId}`))
      .catch(err => console.error("❌ Lỗi rời nhóm:", err));
  }
}
