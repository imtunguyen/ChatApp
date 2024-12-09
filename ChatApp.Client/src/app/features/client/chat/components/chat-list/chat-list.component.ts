import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../../../core/services/auth.service';
import { User } from '../../../../../core/models/user.module';

@Component({
  selector: 'app-chat-list',
  imports: [CommonModule],
  templateUrl: './chat-list.component.html',
  styleUrl: './chat-list.component.scss'
})
export class ChatListComponent implements OnInit {
  users: User[] = [];

  private authService = inject(AuthService);
  constructor() { }
  ngOnInit(): void {
    this.loadUsers();
  }

  selectUser(user: any) {
    console.log('Selected User:', user);
  }
  loadUsers() {
    this.authService.getUsers().subscribe((users) => {
      this.users = users;
    });
  }
}
