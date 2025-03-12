import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzAvatarComponent } from 'ng-zorro-antd/avatar';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { MessageType } from '../../../../core/models/enum/message-type';
import { Message } from '../../../../core/models/message.module';
import { Pagination } from '../../../../shared/models/pagination.module';
import { MessageParams } from '../../../../shared/params/messageParams';
import { User } from '../../../../core/models/user.module';
import { MessageService } from '../../../../core/services/message.service';
import { AuthService } from '../../../../core/services/auth.service';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { Router } from '@angular/router';
import { VideoCallComponent } from "../video-call/video-call.component";
import { SignalRService } from '../../../../core/services/signalr.service';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { GroupService } from '../../../../core/services/group.service';
import { tap } from 'rxjs';
@Component({
  selector: 'app-chat-box',
  imports: [CommonModule, NzAvatarComponent, NzIconModule, FormsModule, NzSpinModule, VideoCallComponent, InfiniteScrollModule],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.scss'
})
export class ChatBoxComponent implements OnInit, OnChanges{
  @Input() selectedUser: any;
  @Input() selectedGroup: any;

  userGroups: User[] = [];
  userCache = new Map<string, any>();
  currentUser: any;
  avatars: { [key: string]: string } = {};
  isVisible: boolean = true;
  isVideoCallVisible: boolean = false;
  
  isEditing: boolean = false;

  //record
  isRecording: boolean = false;
  audioUrl: string | null = null;
  mediaRecorder: MediaRecorder | null = null;
  audioChunks: Blob[] = [];

  //search
  isSearching = false;
  searchText = '';

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

  @ViewChild('messageContainer') messagesContainer!: ElementRef;

  private messageService = inject(MessageService);
  private authService = inject(AuthService);
  private signalRService = inject(SignalRService);
  private groupService = inject(GroupService);

  constructor(private router: Router) {
    this.currentUser = this.authService.getCurrentUser();
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedUser'] && changes['selectedUser'].currentValue) {
      this.selectedGroup = null;
      this.resetValues();
      this.loadMessages();
      this.scrollToBottom();
      console.log("day la selectUser", this.selectedUser)
    } else if (changes['selectedGroup'] && changes['selectedGroup'].currentValue) {
      this.selectedUser = null;
      this.resetValues();
      this.loadMessagesGroup();
      this.scrollToBottom();
      this.loadUserGroups();
      console.log("Đã chọn Group:", this.selectedGroup);
    }
  }
  ngOnInit(): void {
    this.signalRService.newMessage$.subscribe({
      next: (res) => {
        if (res) {
          this.messages.push(res.message);
          this.scrollToBottom();
        }
      }
    });
    
  }
  resetValues(): void {
    this.messages = [];
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
          console.log("Raw API Response:", response);
          this.messages = response.items || [];
          console.log("Final Messages:", this.messages);
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

  toggleVideoCall(){
    this.isVideoCallVisible = !this.isVideoCallVisible;
  }
    
  updateMessage(message: Message) {
    console.log("update message", message)
    if(message.senderId !== this.currentUser.id) return;
    this.originalMessage = message.content;
    this.idMessage = message.id;
    this.newMessage = message.content;
    this.isEditing = true;
  }

  deleteMessage(messageId: number) {
    this.messageService.deleteMessage(messageId).subscribe({
      next: () => {
        this.messages = this.messages.filter(m => m.id !== messageId);
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
  
      if(this.idMessage === 0){
        this.sendMessageToServer(formData);
      } else {
        this.messageService.updateMessage(formData).subscribe({
          next: (response) => {
            const message = response as Message;
            this.messages = this.messages.map(m => m.id === message.id ? message : m);
            this.signalRService.sendMessage(message.content, this.selectedUser.id);
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
    if(this.selectedGroup){
      formData.append('groupId', this.selectedGroup.id);
      this.messageService.addMessage(formData).subscribe({
        next: (response) => {
          const message = response as Message;
          this.messages.push(message);
          this.signalRService.sendMessage(message.content, this.selectedGroup.id);
    
          this.scrollToBottom();
          this.resetMessageInput();
        }
      });
    } else{

      this.messageService.addMessage(formData).subscribe({
        next: (response) => {
          const message = response as Message;
          this.messages.push(message);
          this.signalRService.sendMessage(message.content, this.selectedUser.id);
    
          this.scrollToBottom();
          this.resetMessageInput();
        },
        error: (error) => console.log(error)
      });
    }
    
  }
  
  loadMessages() {
    if (this.loading) return;
    this.loading = true;

    this.messageService
      .getMessagesThread(this.params, this.currentUser.id, this.selectedUser.id)
      .subscribe((result) => {
        if (result.items) {
          this.messages = [...result.items.reverse(), ...this.messages];
        }
        console.log('messages', this.messages);
        this.pagination = result.pagination || {
          currentPage: 1,
          itemPerPage: 10,
          totalItems: 0,
          totalPages: 1,
        };
        this.loading = false;
      });
  }

  loadMessagesGroup() {
    if (this.loading) return;
    this.loading = true;
    
    this.messageService
      .getMessagesGroup(this.params, this.selectedGroup.id)
      .subscribe((result) => {
        if (result.items) {
          this.messages = [...result.items.reverse(), ...this.messages];
          
        }
        console.log('messages', this.messages);
        this.messages.forEach(message => {
          this.getUserById(message.senderId).subscribe({
            next: (user : any) => {
              this.avatars[message.senderId] = user.profilePictureUrl;
            },
            error: (error: any) => {
              console.log(error);
            }
          });
        });
        this.pagination = result.pagination || {
          currentPage: 1,
          itemPerPage: 10,
          totalItems: 0,
          totalPages: 1,
        };
        this.loading = false;
      });
  }

  isImage(message: Message): boolean {
    return message.type === MessageType.Image;
  }
  isVideo(message: Message): boolean {
    return message.type === MessageType.Video;
  }
  isAudio(message: Message): boolean {
    return message.type === MessageType.Audio;
  }

  onFileSelected(event: any) {
    console.log("hhhhhhhhhhhhhh")
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
    this.selectedFiles = [];
  }

  // Group

  loadUserGroups() {
    this.groupService.getUsersByGroup(this.selectedGroup.id).subscribe({
      next: (users) => {
        console.log("UsersGroup:", users);
        this.userGroups = Array.isArray(users) ? users : [];
      },
      error: (error) => {
        console.log(error);
      }
    });
  }


  // getMessagesChatRoom(){
  //   this.messageService.getMessagesChatRoom(this.messageParams, this.selectedChatRoom.id).subscribe({
  //     next: (response) => {
  //       this.messages = response.result as Message[];
  //       this.pagination = response.pagination as Pagination;

  //       this.messages.forEach(message => {
  //         this.authService.getUserById(message.senderId).subscribe({
  //           next: (user) => {
  //             this.avatars[message.senderId] = user.profilePictureUrl;
  //           },
  //           error: (error) => {
  //             console.log(error);
  //           }
  //         });
  //       })


  //     },
  //     error: (error) => {
  //       console.log(error);
  //     }
  //   });
  // }

 

}
