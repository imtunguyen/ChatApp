import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-chat-details',
  imports: [CommonModule],
  templateUrl: './chat-details.component.html',
  styleUrl: './chat-details.component.scss'
})
export class ChatDetailsComponent implements OnInit {
  @Input() user: any;
  ngOnInit(): void {
    console.log("ChatDetailsComponent initialized");
  }
}
