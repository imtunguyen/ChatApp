import { Component } from '@angular/core';
import { ChatListComponent } from "../../components/chat-list/chat-list.component";
import { ChatBoxComponent } from "../../components/chat-box/chat-box.component";
import { ChatDetailsComponent } from "../../components/chat-details/chat-details.component";
import { RouterOutlet } from '@angular/router';
import { TuiRoot } from '@taiga-ui/core';

@Component({
  selector: 'app-chat-page',
  imports: [ChatListComponent, ChatBoxComponent, ChatDetailsComponent],
  templateUrl: './chat-page.component.html',
  styleUrl: './chat-page.component.scss'
})
export class ChatPageComponent {

}
