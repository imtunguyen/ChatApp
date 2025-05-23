import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, ElementRef, inject, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzAvatarComponent, NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { MessageType } from '../../../../core/models/enum/message-type';
import { Message, MessageFile } from '../../../../core/models/message.module';
import { Pagination } from '../../../../shared/models/pagination.module';
import { User } from '../../../../core/models/user.module';
import { MessageService } from '../../../../core/services/message.service';
import { AuthService } from '../../../../core/services/auth.service';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { Router } from '@angular/router';
import { SignalRService } from '../../../../core/services/signalr.service';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { GroupService } from '../../../../core/services/group.service';
import { distinctUntilChanged, Subject, takeUntil, tap } from 'rxjs';
import { FriendShipService } from '../../../../core/services/friendship.service';
import { FriendShipStatus } from '../../../../core/models/enum/friendship-status';
import { ToastrService } from '../../../../shared/services/toastr.service';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { AiService } from '../../../../core/services/ai.service';
@Component({
  selector: 'app-chat-box',
  imports: [CommonModule, NzAvatarComponent, NzIconModule, NzCheckboxModule, FormsModule, NzMenuModule, NzSpinModule, NzAvatarModule, NzModalModule, NzInputModule],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.scss'
})
export class ChatBoxComponent implements OnInit, OnChanges{


  @Input() selectedUser: any;
  @Input() selectedGroup: any;

  users: User[] = [];
  userGroups: User[] = [];
  userCache = new Map<string, any>();
  currentUser: any;
  selectedUsers: any[] = [];
  selectedUserIds: string[] = [];
  avatars: { [key: string]: string } = {};
  isVisible: boolean = true;
  addMemberVisible: boolean = false;
  btnFriendShipText: string = '';
  friendShipStatus: { [userId: string]: FriendShipStatus } = {};
  isCollapsed = false;
  showInfoModal = false;
  isVideoCallVisible: boolean = false;
  isBlocked: boolean = false;
  isEditing: boolean = false;

  //record
  isRecording: boolean = false;
  audioUrl: string | null = null;
  mediaRecorder: MediaRecorder | null = null;
  audioChunks: Blob[] = [];

  //search
  isSearching = false;
  searchText = '';
  searchMember = '';

  @ViewChild('searchBox') searchBox!: ElementRef;


  //message
  idMessage: number = 0;
  originalMessage: string = '';
  messages: Message[] = [];
  pagination: Pagination = {
    currentPage: 1,
    itemPerPage: 10,
    totalItems: 0,
    totalPages: 1,
  };

  params = {
    pageNumber: 1,
    pageSize: 10,
    search: '',
  };
  addPageSize: number = 5;
  loading = false;
  newMessage = '';
  MessageType: typeof MessageType = MessageType;
  selectedFiles: { src: string; file: File}[] = [];

  rolesMap: Map<string, string> = new Map();
  @ViewChild('messageContainer') messagesContainer!: ElementRef;

  private messageService = inject(MessageService);
  private authService = inject(AuthService);
  private signalRService = inject(SignalRService);
  private groupService = inject(GroupService);
  private friendShipService = inject(FriendShipService);
  private toastrService = inject(ToastrService);
  private aiService = inject(AiService);

  constructor(private router: Router, private cdr: ChangeDetectorRef) {
  
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedUser']?.currentValue && this.currentUser?.id && changes['selectedUser'].currentValue.id) {
      this.selectedGroup = null;
      this.resetValues();
  
      setTimeout(() => {
        if (!this.selectedUser) return; 
        this.loadMessages();
        this.scrollToBottom();
        this.loadFriendShipStatus(this.selectedUser.id);
      }, 100);
    } else if (changes['selectedGroup']?.currentValue) {
      this.selectedUser = null;
      this.resetValues();
      this.signalRService.waitForConnection();
      this.signalRService.joinGroup(this.selectedGroup.id);
      setTimeout(() => {
        if (!this.selectedGroup) return;
        this.loadMessagesGroup();
        this.scrollToBottom();
        this.loadRoles();
      }, 100);
    }
  }
  
  
  ngOnInit(): void {
    console.log("[DEBUG] ChatBoxComponent khởi tạo");
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.signalRService.startConnection(this.currentUser.id);
      this.signalRService.newMessage$.subscribe({
        next: (res) => {
          console.log("[DEBUG] Đã nhận tin nhắn qua BehaviorSubject:", res);
          if (!res) {
            console.error("res bị null hoặc undefined!");
          } else {
            // Kiểm tra nếu tin nhắn đã có trong mảng thì không thêm vào
          
              console.log("Tin nhắn hợp lệ, cập nhật giao diện.");
              this.messages.push(res);
              this.cdr.detectChanges(); // Đảm bảo giao diện được cập nhật
              this.scrollToBottom();
            
          }
        }
      });
      
    }
    
    

  }
 
  resetValues(): void {
    this.messages = [];
    this.pagination = {
      currentPage: 1,
      itemPerPage: 10,
      totalItems: 0,
      totalPages: 1,
    };
    this.params = {
      pageNumber: 1,
      pageSize: 10,
      search: '',
    }
   
    this.newMessage = '';
    this.avatars = {};
  }

  onScroll() {
    const element = this.messagesContainer.nativeElement;
    const previousHeight = element.scrollHeight;

    if (element.scrollTop === 0 && !this.loading) {
      if (this.pagination.currentPage < this.pagination.totalPages) {
        this.params.pageNumber++;
        this.loadMessages();
        setTimeout(() => {
          const currentHeight = element.scrollHeight;
          element.scrollTop = currentHeight - previousHeight;
        }, 100);
      }
    }
  }

  

  scrollToBottom(): void {
    const element = this.messagesContainer.nativeElement;
    setTimeout(() => {
      element.scrollTop = element.scrollHeight;
    }, 100);
  }

  toggleSearch() {
    this.isSearching = true;
    setTimeout(() => {
      this.searchBox.nativeElement.focus();
      
      
    });
  }

  getUserById(userId: string) {
    if (this.userCache.has(userId)) {
      return this.userCache.get(userId);
    }
    return this.authService.getUserById(userId).pipe(
      tap(user => this.userCache.set(userId, user)) 
    );
  }

  onSearchChange() {
    if (!this.searchText || this.searchText.trim() === '') {
      this.params.search = "";
      this.params.pageNumber = 1; 
      this.messages = []; 
      this.loadMessages();
      this.scrollToBottom();
      return;
    }
  
    this.params.search = this.searchText;
    this.messageService.getMessagesThread(this.params, this.currentUser.id, this.selectedUser.id)
      .subscribe({
        next: (response) => {
          this.messages = response.items || [];
        },
        error: (error) => {
          console.error("Error fetching messages:", error);
        }
      });
  }

  hideSearch() {
    if(!this.searchText.trim()) {
      this.isSearching = false;
    }
  }

  toggleChatDetail(){
    this.isVisible = !this.isVisible;
  }

    
  updateMessage(message: Message) {
    if(message.senderId !== this.currentUser.id) return;
    this.originalMessage = message.content;
    this.idMessage = message.id;
    this.newMessage = message.content;
    this.isEditing = true;
  }

  cancelEditing() {
    this.isEditing = false;
    this.resetMessageInput();
  }

  deleteMessage(messageId: number) {
    this.messageService.deleteMessage(messageId).subscribe({
      next: () => {
        this.toastrService.showSuccess("Đã thu hồi tin nhắn");
        this.loadMessages();
        this.loadMessagesGroup();
      },
      error: (error) => console.log(error)
    });
  }


  async sendMessage() {
    if (!this.newMessage.trim() && !this.audioUrl && this.selectedFiles.length === 0) {
      return; 
    }
  
    const formData = new FormData();
    formData.append('id', this.idMessage.toString());
    formData.append('content', this.newMessage);
    formData.append('recipientId', this.selectedUser?.id || null);
    formData.append('senderId', this.currentUser.id);
  
    this.selectedFiles.forEach(file => formData.append('files', file.file));
  
    try {
      if (this.audioUrl) {
        const blob = await this.fetchAudioBlob(this.audioUrl);
        formData.append('files', blob, 'recording.wav');
      }
  
      if (this.selectedUser?.id === 'AI') {
        const prompt = this.newMessage.trim();
        this.aiService.askAI(prompt, ).subscribe((response) => {
          const reply = response.message;
          console.log(        "[DEBUG] Phản hồi từ AI:", reply);
          const aiMessage: Message = {
            id: 0,
            content: reply,
            senderId: 'AI',
            recipientId: this.currentUser.id,
            sentAt: new Date(),
            isRead: true,
            updatedAt: new Date(),
            status: '',
            type: MessageType.Document,
            files: [],
            isDeleted: false 
          };
        
          this.messages.push({
            id: 0,
            content: prompt,
            senderId: this.currentUser.id,
            recipientId: 'AI',
            sentAt: new Date(),
            isRead: true,
            updatedAt: new Date(),
            status: '',
            type: MessageType.Document,
            files: [],
            isDeleted: false 
          });
        
          this.messages.push(aiMessage);
          this.scrollToBottom();
          this.resetMessageInput();
        });
        return;
      }
      
  
      if (this.idMessage === 0 && !this.isEditing) {
        this.sendMessageToServer(formData);
      } else {
        this.messageService.updateMessage(formData).subscribe({
          next: (response) => {
            const message = response as Message;
            this.messages = this.messages.map(m => m.id === message.id ? message : m);
            this.signalRService.sendMessage(message);
            this.messageService.notifyMessageUpdate();
            this.isEditing = false;
            this.resetMessageInput();
          },
          error: (error) => console.log(error)
        });
      }
    } catch (error) {
      console.error('Lỗi khi gửi tin nhắn:', error);
    }
  }
  
  private async fetchAudioBlob(audioUrl: string): Promise<Blob> {
    const response = await fetch(audioUrl);
    return response.blob();
  }
  
  private sendMessageToServer(formData: FormData) {
    if (this.selectedGroup) {
      formData.append('groupId', this.selectedGroup.id);
      this.messageService.addMessage(formData).subscribe({
        next: (response) => {
          const message = response as Message;
          this.signalRService.sendMessage(message);
          this.messageService.notifyMessageUpdate();
          this.scrollToBottom();
          this.resetMessageInput();
        }
      });
    } else {
      this.messageService.addMessage(formData).subscribe({
        next: (response) => {
          const message = response as Message;
          this.messages.push(message);
          this.signalRService.sendMessage(message);
          this.messageService.notifyMessageUpdate();
          this.scrollToBottom();
          this.resetMessageInput();
        },
        error: (error) => console.log(error)
      });
    }
  }
  

  loadMessages() {
    if (this.loading || !this.selectedUser?.id || !this.currentUser?.id) return;
    this.loading = true;
  
    this.messageService
      .getMessagesThread(this.params, this.currentUser.id, this.selectedUser.id)
      .subscribe((result) => {
        const newMessages = result.items ?? [];
  
        // Cập nhật lại từng message theo id
        const messagesMap = new Map(this.messages.map(m => [m.id, m]));
  
        newMessages.forEach(newMsg => {
          messagesMap.set(newMsg.id, newMsg); // cập nhật hoặc thêm mới
        });
  
        // Gộp lại và sort theo thời gian (nếu cần)
        this.messages = Array.from(messagesMap.values()).sort((a, b) => 
          new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
        );
  
        this.pagination = result.pagination || {
          currentPage: 1,
          itemPerPage: 10,
          totalItems: 0,
          totalPages: 1,
        };
  
        this.loading = false;
        this.cdr.detectChanges(); // đảm bảo render lại
      });
  }
  
  
  
 

  loadMessagesGroup() {
    if (this.loading || !this.selectedGroup?.id) return;
    this.loading = true;
    this.messageService
      .getMessagesGroup(this.params, this.selectedGroup.id)
      .subscribe((result) => {
        const newMessages = result.items ?? [];
  
        // Dùng Map để cập nhật hoặc thêm message theo id
        const messagesMap = new Map(this.messages.map(m => [m.id, m]));
        newMessages.forEach(newMsg => {
          messagesMap.set(newMsg.id, newMsg); // update hoặc thêm mới
        });
  
        // Cập nhật lại this.messages và sort theo thời gian nếu cần
        this.messages = Array.from(messagesMap.values()).sort((a, b) =>
          new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
        );
  
        // Cập nhật avatar cho các sender mới
        this.messages.forEach(message => {
          if (!this.avatars[message.senderId]) {
            this.getUserById(message.senderId).subscribe({
              next: (user: any) => {
                this.avatars[message.senderId] = user.profilePictureUrl;
              },
              error: (error: any) => {
                console.log(error);
              }
            });
          }
        });
  
        // Pagination
        this.pagination = result.pagination || {
          currentPage: 1,
          itemPerPage: 10,
          totalItems: 0,
          totalPages: 1,
        };
  
        this.loading = false;
        this.cdr.detectChanges(); // đảm bảo cập nhật giao diện
      });
  }
  

  isImage(file: MessageFile): boolean {
    return file.fileType === MessageType.Image;
  }
  isVideo(file: MessageFile): boolean {
    return file.fileType === MessageType.Video;
  }
  isAudio(file: MessageFile): boolean {
    return file.fileType === MessageType.Audio;
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

  onPaste(event: ClipboardEvent){
    const items = Array.from(event.clipboardData?.items || []);
    items.forEach((item) => {
      if (item.type.indexOf('image') !== -1) {
        const file = item.getAsFile();
        const reader = new FileReader();
        if(file){
          reader.onload = (e: any) => {
            this.selectedFiles.push({
              src: e.target.result,
              file,
            });
          };
          reader.readAsDataURL(file);
        }
      }
    });
  }

  removeImage(index: number) {
    this.selectedFiles.splice(index, 1);
  }

  startRecording() {
    this.isRecording = true;
    navigator.mediaDevices.getUserMedia({ audio: true }).then(stream => {
        this.mediaRecorder = new MediaRecorder(stream);
        this.audioChunks = [];

        this.mediaRecorder.ondataavailable = (event) => {
            this.audioChunks.push(event.data);
        };

        this.mediaRecorder.onstop = () => {
            const audioBlob = new Blob(this.audioChunks, { type: 'audio/wav' });
            this.audioUrl = URL.createObjectURL(audioBlob);
        };

        this.mediaRecorder.start();
    }).catch(error => console.error("Microphone access denied:", error));
}


  stopRecording() {
    this.isRecording = false;
    if (this.mediaRecorder) {
        this.mediaRecorder.stop();
    }
  }


  resetMessageInput() {
    this.newMessage = '';
    this.originalMessage = '';
    this.idMessage = 0;
    this.selectedFiles = [];
    this.audioUrl = '';
  }

  // Group

  loadRoles() {
    this.authService.getRoles().subscribe({
      next: (roles) => {
        this.rolesMap = new Map(); 
        roles.forEach(role => {
          this.rolesMap.set(role.id, role.name);
        });
        this.loadUserGroups(); 
      },
      error: (error) => {
        console.log("Error loading roles:", error);
      }
    });
  }
  
  
  // Load danh sách user trong nhóm
  loadUserGroups() {
    this.groupService.getUsersByGroup(this.selectedGroup.id).subscribe({
      next: (users) => {
        console.log("UsersGroup:", users);
        this.userGroups = Array.isArray(users) ? users.filter(user => !user.isRemoved) : [];
        this.userGroups.forEach(user => {
          user.roleName = this.rolesMap.get(user.roleId) || "Unknown";
        });
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  showUserProfile(user: User) {
    this.showInfoModal = true;
  }
  showAddMemberModal(groupId: number) {
    this.loadUsers();
    this.groupService.getUsersByGroup(groupId).subscribe({
      next: (usersInGroup) => {
        this.users = this.users.map(user => {
          const isMember = Array.isArray(usersInGroup) && usersInGroup.some(groupUser => groupUser.id === user.id);
          return { ...user, isMember }; 
        });
      }
    });
    
    this.addMemberVisible = true;
  }
  loadUsers() {
    this.authService.getUsers().subscribe({
      next: (users) => {
        this.users = users.filter((user) => user.id !== this.currentUser.id);
      },
      error: (error) => {
        console.log("Error loading users:", error);
      }
    });

  }
  handleCancel() {
    console.log('Button cancel clicked!');
    this.showInfoModal = false;
  }

  handleMemberCancel() {
    this.addMemberVisible = false;
  }

  getFriendShipStatus(userId: string) {
    this.friendShipService.getFriendShips(this.currentUser.id, userId).subscribe({
      next: (friendShip) => {
        this.friendShipStatus[userId] = friendShip?.status || FriendShipStatus.None;
        if(friendShip?.status === FriendShipStatus.None){
          this.btnFriendShipText = 'Kết bạn';
        }
        else if(friendShip?.status === FriendShipStatus.Accepted){
          this.btnFriendShipText = 'Hủy kết bạn';
        }
      },
      error: (error) => {
        this.friendShipStatus[userId] = FriendShipStatus.None;
      }
    });
  }

  updateFriendShip(userId: string) {
   this.friendShipService.updateFriendShip(this.currentUser.id, userId, FriendShipStatus.None).subscribe({
     next: (response) => {
       console.log("Remove friend response:", response);
       this.toastrService.showSuccess("Đã xóa bạn bè");
     },
      error: (error) => {
        console.log("Error removing friend:", error);
      }
    });
  }
  blockUser(userId: string) {
    this.friendShipService.updateFriendShip(this.currentUser.id, userId, FriendShipStatus.Blocked).subscribe({
      next: (response) => {
        console.log("Block user response:", response);
        this.toastrService.showSuccess("Đã chặn người dùng");
        this.loadFriendShipStatus(userId);
        this.friendShipService.notifyFriendShipUpdate();
      },
      error: (error) => {
        console.log("Error blocking user:", error);
      },
    });
  }

  unblockUser(userId: string) {
    this.friendShipService.getFriendShips(this.currentUser.id, userId).subscribe({
      next: (friendShip) => {
       
        this.friendShipService.updateFriendShip(this.currentUser.id, userId, FriendShipStatus.Accepted).subscribe({
          next: (response) => {
            this.toastrService.showSuccess("Đã bỏ chặn người dùng");
            this.btnFriendShipText = 'Chặn';
            this.loadFriendShipStatus(userId);
            this.friendShipService.notifyFriendShipUpdate();
          },
          error: (error) => {
            console.log("Error unblocking user:", error);
          },
        });
      },
      error: (error) => {
        console.log("Error fetching friendship status:", error);
        this.toastrService.showError("Lỗi khi kiểm tra trạng thái bạn bè.");
      }
    });
   
  }

  loadFriendShipStatus(userId: string) {
    this.friendShipService.getFriendShips(this.currentUser.id, userId).subscribe({
      next: (friendShip) => {
        this.friendShipStatus[userId] = friendShip?.status || FriendShipStatus.None;
       
        if(friendShip?.status === FriendShipStatus.Blocked){
          this.isBlocked = true;
          this.btnFriendShipText = 'Bỏ chặn';
        }
        else{
          this.isBlocked = false;
          this.btnFriendShipText = 'Chặn';
        }
      }
    });
  }

  toggleUser(user: User, isChecked: boolean) {
    user.isSelected = isChecked;
  
    if (isChecked) {
      if (!this.selectedUsers.some(u => u.id === user.id)) {
        this.selectedUsers.push(user);
        this.selectedUserIds.push(user.id);
      }
    } else {
      this.selectedUsers = this.selectedUsers.filter(u => u.id !== user.id);
      this.selectedUserIds = this.selectedUserIds.filter(id => id !== user.id);
    }

  }
  

  addMember() {
    const userGroups = this.selectedUsers.map(user => ({
      groupId: this.selectedGroup.id,
      userId: user.id,
      roleId: '0'
    }));
  
    this.groupService.addMemberToGroup(userGroups).subscribe({
      next: (response) => {
        this.addMemberVisible = false;
        this.loadUserGroups();
      },
      error: (error) => {
        console.log("Error adding members:", error);
      }
    });
  }
  removeMemberFromGroup(userId: string) {
    const groupOwners = this.userGroups.filter(user => user.roleName === "GroupOwner");
  
    if (groupOwners.length === 1 && groupOwners[0].id === userId) {
      this.toastrService.showError("Nhóm cần ít nhất một GroupOwner. Không thể rời nhóm.");
      return;
    }
  
    // Nếu hợp lệ, thực hiện xóa thành viên
    this.groupService.removeMember(userId, this.selectedGroup.id).subscribe({
      next: (response) => {
        if(userId === this.currentUser.id){
          this.toastrService.showSuccess("Đã rời nhóm");
          this.selectedGroup = null; 
          this.router.navigate(['/chat']);
          this.groupService.notifyGroupUpdate();
        }
        else{
          this.toastrService.showSuccess("Đã xóa thành viên");
        }
       
        // Load lại danh sách userGroups sau khi xóa
        this.loadUserGroups();
        this.groupService.notifyGroupUpdate();
      },
      error: (error) => {
        console.log("Error removing member:", error);
      }
    });
  }
  

  updateRole(member: any){
    const userGroup = {
      groupId: this.selectedGroup.id,
      userId: member.id,
      roleId: member.roleId,
      isRemoved: member.isRemoved,
      removedAt: member.removedAt
    };
    this.groupService.updateRole(userGroup).subscribe({
      next: (response) => {
        this.loadUserGroups();
        this.toastrService.showSuccess("Đã cập nhật vai trò");
      },
      error: (error) => {
        console.log("Error updating role:", error);
      }
    });
  }

  deleteGroup() {
    this.groupService.deleteGroup(this.selectedGroup.id).subscribe({
      next: (response) => {
        this.toastrService.showSuccess("Đã xóa nhóm");
        this.router.navigate(['/chat']);
      },
      error: (error) => {
        console.log("Error deleting group:", error);
      }
    });
  }

 

}
