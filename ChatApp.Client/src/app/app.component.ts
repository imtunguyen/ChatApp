import { TuiRoot } from "@taiga-ui/core";
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChatListComponent } from "./features/client/chat/components/chat-list/chat-list.component";
import { ChatDetailsComponent } from "./features/client/chat/components/chat-details/chat-details.component";
import { ChatBoxComponent } from "./features/client/chat/components/chat-box/chat-box.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TuiRoot],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ChatApp.Client';
}
