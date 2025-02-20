import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormBuilder, FormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { User } from '../../../../core/models/user.module';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { AuthService } from '../../../../core/services/auth.service';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { FriendShipService } from '../../../../core/services/friendship.service';
import { ToastrService } from '../../../../shared/services/toastr.service';
import { FriendShipStatus } from '../../../../core/models/enum/friendship-status';
import { Observable } from 'rxjs';
@Component({
  selector: 'app-chat-list',
  imports: [NzInputModule, FormsModule, CommonModule, NzAvatarModule, NzIconModule, NzModalModule],
  templateUrl: './chat-list.component.html',
  styleUrl: './chat-list.component.scss'
})
export class ChatListComponent {
  @Output() userSelected = new EventEmitter<any>();
  @Output() groupSelected = new EventEmitter<any>();
  users: User[] = [];
  currentUser: any;
  protected name = '';
  isVisible = false;
  friendShipStatus: { [userId: string]: FriendShipStatus } = {};

  private authService = inject(AuthService);
  private friendShipService = inject(FriendShipService);
  private toastService = inject(ToastrService);
  constructor(private fb: FormBuilder) {
    this.currentUser = this.authService.getCurrentUser();
   }
  ngOnInit(): void {
    this.loadUsers();
    this.loadFriendShipStatus();
    
    //this.loadChatRooms();
  }

  loadFriendShipStatus() {
    this.users.forEach((user) => {
      this.friendShipService.getFriendShips(this.currentUser.id, user.id).subscribe(
        (friendShip) => {
          this.friendShipStatus[user.id] = friendShip?.status || FriendShipStatus.None;
        },
        (err) => {
          this.friendShipStatus[user.id] = FriendShipStatus.None;
        });
    });
  }

  loadUsers() {
    this.authService.getUsers().subscribe((users) => {
      this.users = users.filter((user) => user.id !== this.currentUser.id);
    });

  }

  showAddFriendModal() {
    this.isVisible = true;
  }

  handleOk(): void {
    console.log('Button ok clicked!');
    this.isVisible = false;
  }

  handleCancel(): void {
    console.log('Button cancel clicked!');
    this.isVisible = false;
  }

  selectUser(user: any) {
    console.log('Selected User:', user);
    this.userSelected.emit(user);
  }

  showAddGroupModal() {
    
  }

  addFriend(userId: string) {
    console.log('Add friend:', userId);
    console.log('Current user:', this.currentUser);
    this.friendShipService.addFriendShip(this.currentUser.id, userId).subscribe(
      (res) => {
        this.friendShipStatus[userId] = FriendShipStatus.Pending;
        this.toastService.showSuccess('Đã gửi lời mời kết bạn');
      },
      (err) => {
        this.toastService.showError('Lỗi khi gửi lời mời kết bạn');
      }
    );
  }

  cancelFriendRequest(userId: string): void {
    this.friendShipService.getFriendShips(this.currentUser.id, userId).subscribe(
      (friendShip) => {
        friendShip.status = FriendShipStatus.None;
        this.friendShipService.updateFriendShip(this.currentUser.id, userId, friendShip.status).subscribe(
          (res) => {
            this.friendShipStatus[userId] = FriendShipStatus.None;
            this.toastService.showSuccess('Đã hủy lời mời kết bạn');
          },
          (err) => {
            this.toastService.showError('Lỗi khi hủy lời mời kết bạn');
          }
        );
      },
      (err) => {
        this.toastService.showError('Lỗi khi hủy lời mời kết bạn');
      });
  }

  // Hàm kiểm tra trạng thái friendship
  getFriendShipStatus(userId: string): FriendShipStatus {
    return this.friendShipStatus[userId] || FriendShipStatus.None;
  }
}
