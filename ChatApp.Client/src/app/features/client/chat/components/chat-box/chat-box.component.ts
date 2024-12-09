import { CommonModule } from '@angular/common';
import { Component, CUSTOM_ELEMENTS_SCHEMA, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat-box',
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ChatBoxComponent implements OnInit {
  ngOnInit(): void {
    console.log("ChatBoxComponent initialized");
  }
  @Input() selectedUser: any;
  messages: { text: string; isSent: boolean }[] = [];
  newMessage = '';

  sendMessage() {
    if (this.newMessage.trim()) {
      this.messages.push({ text: this.newMessage, isSent: true });
      this.newMessage = '';
    }
  }
}
