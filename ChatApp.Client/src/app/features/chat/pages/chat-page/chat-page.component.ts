import { Component } from '@angular/core';
import { ChatBoxComponent } from "../../components/chat-box/chat-box.component";
import { ChatListComponent } from "../../components/chat-list/chat-list.component";

@Component({
  selector: 'app-chat-page',
  imports: [ChatBoxComponent, ChatListComponent],
  templateUrl: './chat-page.component.html',
  styleUrl: './chat-page.component.scss'
})
export class ChatPageComponent {
  selectedUser: any;
  selectedGroup: any;
  
  onUserSelected(user: any) {
    this.selectedUser = user;
  }

  onGroupSelected(group: any) {
    this.selectedGroup = group;
  }
  
}
