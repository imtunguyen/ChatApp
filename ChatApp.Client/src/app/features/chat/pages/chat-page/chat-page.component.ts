import { Component } from '@angular/core';
import { ChatBoxComponent } from "../../components/chat-box/chat-box.component";
import { ChatListComponent } from "../../components/chat-list/chat-list.component";
import { NzMenuModule } from 'ng-zorro-antd/menu';
@Component({
  selector: 'app-chat-page',
  standalone: true,
  imports: [ChatBoxComponent, ChatListComponent, NzMenuModule],
  templateUrl: './chat-page.component.html',
  styleUrl: './chat-page.component.scss'
})
export class ChatPageComponent {
  selectedUser: any;
  selectedGroup: any;
  
  onUserSelected(user: any) {
    this.selectedGroup = null;
    this.selectedUser = user;
  }

  onGroupSelected(group: any) {
    this.selectedUser = null;
    this.selectedGroup = group;
  }
  
}
