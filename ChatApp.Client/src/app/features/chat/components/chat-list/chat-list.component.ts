import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, inject, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { User } from '../../../../core/models/user.module';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { AuthService } from '../../../../core/services/auth.service';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { FriendShipService } from '../../../../core/services/friendship.service';
import { ToastrService } from '../../../../shared/services/toastr.service';
import { FriendShipStatus } from '../../../../core/models/enum/friendship-status';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { UserStatusService } from '../../../../core/services/user-status.service';
import { SignalRService } from '../../../../core/services/signalr.service';
import { Subscription } from 'rxjs';
@Component({
  selector: 'app-chat-list',
  imports: [NzInputModule, FormsModule, CommonModule, NzAvatarModule, NzIconModule, NzModalModule, ReactiveFormsModule, NzCheckboxModule],
  templateUrl: './chat-list.component.html',
  styleUrl: './chat-list.component.scss'
})
export class ChatListComponent {
  @Output() userSelected = new EventEmitter<any>();
  @Output() groupSelected = new EventEmitter<any>();
  users: User[] = [];
  selectedUsers: any[] = [];
  onlineUsers: string[] = [];
  currentUser: any;

  newMessageFrom: string | null = null;
  protected value = '';
  protected name = '';
  groupForm!: FormGroup;
  isFriendVisible = false;
  isGroupVisible = false;
  friendShipStatus: { [userId: string]: FriendShipStatus } = {};
  selectedFiles: { src: string; file: File}[] = [];
  search: string = '';
  protected onModelChange(value: string): void {
    this.value = value;
    
  }
  private onlineUsersSubscription!: Subscription;


  private authService = inject(AuthService);
  private friendShipService = inject(FriendShipService);
  private toastService = inject(ToastrService);
  private userStatusService = inject(UserStatusService);
  private signalR = inject(SignalRService);
  constructor(private fb: FormBuilder, private cdr: ChangeDetectorRef) {
    this.currentUser = this.authService.getCurrentUser();
   }
  ngOnInit(): void {
    this.signalR.startConnection();
    if (!this.currentUser) {
      console.warn("Current user is undefined, waiting for authentication...");
      return;
    }
    this.loadUsers();
    this.loadFriendShipStatus();
    this.groupForm = this.initializeForm();
    //this.loadChatRooms();

    if (this.onlineUsersSubscription) {
      this.onlineUsersSubscription.unsubscribe();
    }

    this.signalR.onlineUsers$.subscribe((users : any) => {
      console.log("SignalR Online Users:", users);
      this.onlineUsers = users; 
      this.users.forEach((user) => {
        user.isOnline = users.includes(user.id);
      });
      this.cdr.detectChanges();
    });


    this.authService.getOnlineUsers();
  }
  ngOnDestroy(): void {
    if (this.onlineUsersSubscription) {
      this.onlineUsersSubscription.unsubscribe();
    }
  }

  initializeForm() {
    return this.fb.group({
      name: [''],
      users: ['']
    });
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

  toggleUser(user: User, isChecked: boolean) {
    if (isChecked) {
      if (!this.selectedUsers.includes(user)) {
        this.selectedUsers.push(user);
      }
    } else {
      this.selectedUsers = this.selectedUsers.filter(u => u.id !== user.id);
    }
  }

  onFileSelected(event: any) {
    const files: FileList = event.target.files;
    const newFiles: { src: string; file: File}[] = [];

    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      const reader = new FileReader();
      reader.onload = (e: any) => {
          newFiles.push({
            src: e.target.result,
            file,
          });

        if (i === files.length - 1) {
          this.selectedFiles = [...this.selectedFiles, ...newFiles];
        }
      };
      reader.readAsDataURL(file);

    }
  }

  showAddFriendModal() {
    this.isFriendVisible = true;
    this.loadFriendShipStatus();
  }

  handleOk(): void {
    console.log('Button ok clicked!');
    this.isFriendVisible = false;
    this.isGroupVisible = false;
  }

  handleCancel(): void {
    console.log('Button cancel clicked!');
    this.isFriendVisible = false;
    this.isGroupVisible = false;

  }

  selectUser(user: any) {
    console.log('Selected User:', user);
    this.userSelected.emit(user);
  }

  showAddGroupModal() {
    this.isGroupVisible = true;
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

  addGroup() {
    console.log('Add group:', this.groupForm.value);
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
