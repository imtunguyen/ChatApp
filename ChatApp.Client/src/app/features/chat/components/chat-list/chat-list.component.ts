import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, inject, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
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
import { SignalRService } from '../../../../core/services/signalr.service';
import { BehaviorSubject, distinctUntilChanged, filter, forkJoin, of, Subject, Subscription, switchMap, take, takeUntil, tap } from 'rxjs';
import { GroupService } from '../../../../core/services/group.service';
import { Group } from '../../../../core/models/group.module';
import { FriendShip } from '../../../../core/models/friendship.module';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { MessageService } from '../../../../core/services/message.service';
@Component({
  selector: 'app-chat-list',
  imports: [NzInputModule, NzTabsModule, NzSelectModule, FormsModule, CommonModule, NzAvatarModule, NzIconModule, NzModalModule, ReactiveFormsModule, NzCheckboxModule],
  templateUrl: './chat-list.component.html',
  styleUrl: './chat-list.component.scss'
})
export class ChatListComponent {
  @Output() userSelected = new EventEmitter<any>();
  @Output() groupSelected = new EventEmitter<any>();
  users: User[] = [];
  allUsers: User[] = [];
  groups: Group[] = [];
  selectedUsers: any[] = [];
  selectedUserIds: string[] = [];
  onlineUsers: string[] = [];
  currentUser: any;
  userGroups: User[] = [];
  friendsList: User[] = [];
  blockedList: User[] = [];
  filteredUsers: User[] = [];
  filteredFriendsList: User[] = [];
  filteredGroups: Group[] = [];
  displayedUsers: User[] = [];
  currentIndex = 0;

  lastMessage: any;
  newMessageFrom: string | null = null;
  protected value = '';
  
  groupForm!: FormGroup;
  isSelectedUser = false;
  isFriendVisible = false;
  isGroupVisible = false;
  friendShipStatus: { [userId: string]: FriendShipStatus } = {};
  selectedFile: File | null = null;
  previewImage: string | ArrayBuffer | null = null;
  searchText: string = '';
  selectedCategory = 'friends'; // Mặc định chọn "Bạn bè"
  categoryOptions = [
    { value: 'friends', label: 'Bạn bè' },
    { value: 'blocked', label: 'Đã chặn' }
  ];
  search: string = '';
  name: string = '';
  selectedTabIndex = 0;
  private onlineUsersSubscription!: Subscription;

  private destroy$ = new Subject<void>();
  private authService = inject(AuthService);
  private friendShipService = inject(FriendShipService);
  private messageService = inject(MessageService);
  private groupService = inject(GroupService);
  private toastService = inject(ToastrService);
  private signalRService = inject(SignalRService);
  constructor(private fb: FormBuilder, private cdr: ChangeDetectorRef) {
   }
  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.currentUser = this.authService.getCurrentUser();
    }
    this.signalRService.startConnection(this.currentUser.id);
    if (!this.currentUser) {
      console.warn("Current user is undefined, waiting for authentication...");
      return;
    }


    // Chờ kết nối thành công mới load online users
    this.signalRService.connectionEstablished$
      .pipe(
        filter(established => established),
        take(1)
      )
      .subscribe(() => {
        this.loadOnlineUsers();
      });
    this.loadUsers();
    this.loadFriendShipStatus();
   
    this.groupForm = this.initializeGroupForm();
    this.groupService.groupUpdate.subscribe(() => {
      this.loadGroups();
    });
    
    this.friendShipService.friendShipUpdate.subscribe(() => {
      this.refreshFriendsAndBlockedList();
      this.selectedTabIndex = 0;
    });
    
    this.messageService.messageUpdate.subscribe(() => {
      this.refreshFriendsAndBlockedList();
    });

    this.signalRService.newMessage$.subscribe({
      next: (res) => {
       this.refreshFriendsAndBlockedList();
      }
    });
    

    if (this.onlineUsersSubscription) {
      this.onlineUsersSubscription.unsubscribe();
    }

    

    this.authService.getOnlineUsers();
  }
  ngOnDestroy(): void {
    if (this.onlineUsersSubscription) {
      this.onlineUsersSubscription.unsubscribe();
    }
    this.destroy$.next();
   this.destroy$.complete();
  }

  private refreshFriendsAndBlockedList() {
    this.loadFriends();
    this.loadGroups();
    this.loadBlockedList();
  }
  
  loadOnlineUsers() {
    this.signalRService.onlineUsers$
  .pipe(distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)))
  .subscribe((users: string[]) => {
    this.onlineUsers = users;
    this.filteredFriendsList.forEach((user) => {
      user.isOnline = users.includes(user.id);
    });
    this.cdr.detectChanges();
  });
  }

  onCategoryChange(value: string) {
    this.selectedCategory = value;
    if(value === 'blocked') {
      this.filteredFriendsList = [...this.blockedList];
    }
    else {
      this.filteredFriendsList = [...this.friendsList];
    }
  }

  initializeGroupForm() {
    return this.fb.group({
      
      name: ['', Validators.required],
      avatarUrl: [null],
      userIds: this.fb.array([]),
      creatorId : this.currentUser.id
    });
  }

  loadFriendShipStatus() {
    this.filteredUsers.forEach((user) => {
      this.friendShipService.getFriendShips(this.currentUser.id, user.id).subscribe(
        (friendShip) => {
          this.friendShipStatus[user.id] = friendShip?.status || FriendShipStatus.None;
        },
        (err) => {
          this.friendShipStatus[user.id] = FriendShipStatus.None;
        });
    });
  }

  getLastMessage(senderId: string, recipientId: string) {
    this.messageService.getLastMessage(senderId, recipientId).subscribe((message) => {
      if (message) {
        this.lastMessage = message.senderId === this.currentUser.id ? message.recepientId : message.senderId;
      } else {
        this.lastMessage = null;
      }
    });
  }

  getLastMessageGroup(groupId: number) {
    this.messageService.getLastMessageGroup(groupId).subscribe((message) => {
      if (message) {
        this.lastMessage = message.senderId === this.currentUser.id ? message.recepientId : message.senderId;
      } else {
        this.lastMessage = null;
      }
    });
  }

  loadUsers() {
    this.filteredUsers = [];
    this.authService.getUsers().subscribe((users) => {
      const allUsers = users.filter((user) => user.id !== this.currentUser.id);

      const notFriends = allUsers.filter(u => this.getFriendShipStatus(u.id) === FriendShipStatus.None);
      const friends = allUsers.filter(u => this.getFriendShipStatus(u.id) !== FriendShipStatus.None);

      this.filteredUsers = [...notFriends, ...friends];
      this.currentIndex = 0;
      // this.users =  users.filter((user) => user.id !== this.currentUser.id);;
      // this.filteredUsers = [...this.users];
   
      this.updateDisplayedUsers();
      this.loadFriendShipStatus();
    });

  }

  updateDisplayedUsers() {
    this.displayedUsers = this.filteredUsers.slice(this.currentIndex, this.currentIndex + 4);
  }

  removeUser(userId: string) {
    const index = this.filteredUsers.findIndex(user => user.id === userId);
    if (index > -1) {
      this.filteredUsers.splice(index, 1);
      if(this.displayedUsers.length < 4 && this.currentIndex +4 <= this.filteredUsers.length) {
        this.updateDisplayedUsers();
      }
      else{
        this.displayedUsers = this.filteredUsers.slice(this.currentIndex, this.currentIndex + 4);
      }
    }
  }

  onSearchUserChange() {
    const keyword = this.name?.trim().toLowerCase();
  
    if (!keyword) {
      // Nếu input rỗng thì reset lại danh sách gốc
      this.loadUsers();
      return;
    }
  
    // Nếu có từ khóa thì tìm kiếm trong danh sách đã lọc
    const filtered = this.filteredUsers.filter(user =>
      user.fullName.toLowerCase().includes(keyword) || user.email.toLowerCase().includes(keyword)
    );
  
    const notFriends = filtered.filter(u => this.getFriendShipStatus(u.id) === 0);
    const others = filtered.filter(u => this.getFriendShipStatus(u.id) !== 0);
  
    this.filteredUsers = [...notFriends, ...others];
    this.currentIndex = 0;
    this.updateDisplayedUsers();
  }
  

  loadBlockedList() {
    this.blockedList = [];
    this.friendShipService.getFriendShipsByUser(this.currentUser.id, FriendShipStatus.Blocked).subscribe(
      (res: FriendShip[]) => {
        res.forEach(friendship => {
          if (friendship.requesterId === this.currentUser.id) {
            this.authService.getUserById(friendship.addresseeId).subscribe(
              (user: User) => {
                this.blockedList= [...this.blockedList, user];
              }
            );
          }
        });
      }
    );
  }

  loadFriends() {
    this.friendsList = [];
    this.filteredFriendsList = [];
  
    this.friendShipService.getFriends(this.currentUser.id).subscribe(
      (res: FriendShip[]) => {
        const requests = res.map(friendship => {
          const userId = (friendship.requesterId === this.currentUser.id)
            ? friendship.addresseeId
            : friendship.requesterId;
  
          return this.authService.getUserById(userId).pipe(
            tap((user: User) => {
              user.lastMessage = '';
              this.friendsList.push(user);
            }),
            switchMap((user: User) =>
              this.messageService.getLastMessage(this.currentUser.id, user.id).pipe(
                tap((message) => {
                  if (message) {
                    // Cờ thông báo nếu có tin chưa đọc
                    if (!message.isRead && message.senderId !== this.currentUser.id) {
                      user.hasNewMessage = true;
                    }
  
                    // Gán nội dung tin nhắn
                    user.lastMessage = (message.senderId === this.currentUser.id)
                      ? `Bạn: ${message.content}`
                      : message.content;
  
                    // Gán thời gian tin nhắn cuối
                    user.lastMessageTime = new Date(message.sentAt);
                  } else {
                    user.lastMessage = "Không có tin nhắn";
                    user.lastMessageTime = new Date(0); // Giá trị thấp nhất để sort
                  }
                })
              )
            )
          );
        });
  
        forkJoin(requests).subscribe(() => {
          // Sắp xếp danh sách theo thời gian tin nhắn mới nhất
          this.filteredFriendsList = [...this.friendsList].sort((a, b) =>
            b.lastMessageTime.getTime() - a.lastMessageTime.getTime()
          );
        });
      }
    );
  }
  
  


  loadGroups() {
    this.groupService.getGroupsByUser(this.currentUser.id).subscribe((groups) => {
      this.groups = groups;
      this.filteredGroups = [...this.groups];
      groups.forEach((group) => {
        this.loadUserGroups(group.id);
        this.messageService.getLastMessageGroup(group.id).subscribe((message) => {
          if (message) {
            // Cờ thông báo nếu có tin chưa đọc
            if (!message.isRead && message.senderId !== this.currentUser.id) {
              group.hasNewMessage = true;
            }
            // Gán nội dung tin nhắn
            group.lastMessage = (message.senderId === this.currentUser.id)
              ? `Bạn: ${message.content}`
              : message.content;
  
            // Gán thời gian tin nhắn cuối
            group.lastMessageTime = new Date(message.sentAt);
          } else {
            group.lastMessage = "Không có tin nhắn";
            group.lastMessageTime = new Date(0); // Giá trị thấp nhất để sort
          }
        });
      });
    });
  }

  loadUserGroups(groupId : number) {
    this.groupService.getUsersByGroup(groupId).subscribe({
      next: (users) => {
        this.userGroups = Array.isArray(users) ? users : [];
      },
      error: (error) => {
        console.log(error);
      }
    });
  }


  toggleUser(user: User) {
    
    if (user.isSelected) {
      if (!this.selectedUsers.some(u => u.id === user.id)) {
        this.selectedUsers.push(user);
        this.selectedUserIds.push(user.id);
      }
    } else {
      this.selectedUsers = this.selectedUsers.filter(u => u.id !== user.id);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.selectedFile = input.files[0];
  
      const reader = new FileReader();
      reader.onload = (e) => {
        this.previewImage = e.target?.result ?? null;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  showAddFriendModal() {
    this.isFriendVisible = true;
    this.loadUsers();
    
  }

  handleOk(): void {
    this.isFriendVisible = false;
    this.isGroupVisible = false;
  }

  handleCancel(): void {
    this.isFriendVisible = false;
    this.isGroupVisible = false;

  }

  selectUser(user: any) {
    user.hasNewMessage = false; 
    
    this.messageService.getLastMessage(this.currentUser.id, user.id).subscribe((message) => {
      if(message.senderId == this.currentUser.id) {
        return;
      } else {
        this.messageService.markAsRead(message.id).subscribe({
        
          next: () => console.log("Messages marked as read"),
          error: err => console.error("Error marking messages as read", err)
        });
      }
      
    }
  )
    this.userSelected.emit(user);
  }

  selectGroup(group: any) {
    this.groupSelected.emit(group);
  }

  showAddGroupModal() {
    this.isGroupVisible = true;
  }
  onSearchChange() {
    if(!this.searchText) {
      this.filteredFriendsList = [...this.friendsList];
      this.filteredGroups = [...this.groups];
    } else {
      this.filteredFriendsList = this.friendsList.filter((user) => user.fullName.toLowerCase().includes(this.searchText.toLowerCase()));
      this.filteredGroups = this.groups.filter((group) => group.name.toLowerCase().includes(this.searchText.toLowerCase()));
    }
  }

  

  addFriend(userId: string) {
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
    if(this.groupForm.invalid) {
      this.toastService.showError('Vui lòng nhập tên nhóm');
      return;
    }
    const formData = new FormData();
    formData.append('name', this.groupForm.value.name);
    this.selectedUserIds.forEach((userId) => {
      formData.append('userIds', userId);
    });
    formData.append('avatarUrl', this.selectedFile as Blob);
    formData.append('creatorId', this.currentUser.id);
    this.groupService.addGroup(formData).subscribe({
      next: (response) => {
        this.toastService.showSuccess('Tạo nhóm thành công');
        this.isGroupVisible = false;
      },
      error: (error) => {
        console.error('Tạo nhóm thất bại', error);
      },
    });
  }

  cancelFriendRequest(userId: string): void {
    this.friendShipService.getFriendShips(this.currentUser.id, userId).subscribe(
      (friendShip) => {
        if (friendShip) {
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
        } else {
          this.toastService.showError('Không tìm thấy thông tin kết bạn');
        }
      },
          (res) => {
            this.friendShipStatus[userId] = FriendShipStatus.None;
            this.toastService.showSuccess('Đã hủy lời mời kết bạn');
          },
        );
  }

  // Hàm kiểm tra trạng thái friendship
  getFriendShipStatus(userId: string): FriendShipStatus {
    return this.friendShipStatus[userId] || FriendShipStatus.None;
  }
}
