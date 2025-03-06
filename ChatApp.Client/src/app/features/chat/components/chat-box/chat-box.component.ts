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
@Component({
  selector: 'app-chat-box',
  imports: [CommonModule, NzAvatarComponent, NzIconModule, FormsModule, NzSpinModule, VideoCallComponent],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.scss'
})
export class ChatBoxComponent implements OnInit, OnChanges{
  @Input() selectedUser: any;
  @Input() selectedChatRoom: any;

  userChatRooms: User[] = [];
  currentUser: any;
  avatars: { [key: string]: string } = {};
  isVisible: boolean = true;
  isVideoCallVisible: boolean = false;
  loading: boolean = false;

  //record
  isRecording: boolean = false;
  audioUrl: string | null = null;
  mediaRecorder: MediaRecorder | null = null;
  audioChunks: Blob[] = [];


  //message
  
  messages: Message[] = [];
  messageParams = new MessageParams();
  pagination: Pagination = { currentPage: 1, itemsPerPage: 5, totalItems: 0, totalPages: 0 };
  newMessage = '';
  MessageType: typeof MessageType = MessageType;
  selectedFiles: { src: string; file: File}[] = [];

  @ViewChild('messageContainer') messageContainer!: ElementRef;

  private messageService = inject(MessageService);
  private authService = inject(AuthService);
  private signalRService = inject(SignalRService);

  constructor(private router: Router) {
    this.currentUser = this.authService.getCurrentUser();
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedUser'] && changes['selectedUser'].currentValue) {
      this.resetValues();
      this.getMessagesThread();
      this.scrollToBottom();
      console.log("day la selectUser", this.selectedUser)
    } else if (changes['selectedChatRoom'] && changes['selectedChatRoom'].currentValue) {
      this.resetValues();
      this.getMessagesChatRoom();
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

  onScroll(){
    const element = this.messageContainer.nativeElement;
    const previousHeight = element.scrollHeight;

    if(element.scrollTop === 0 && !this.loading){
      if(this.pagination.currentPage < this.pagination.totalPages){
        this.messageParams.pageNumber++;
        this.getMessagesThread();
        setTimeout(() => {
          const currentHeight = element.scrollHeight;
          element.scrollTop = currentHeight - previousHeight;
        }, 100);
      }
    }
  }

  scrollToBottom(): void {
    const element = this.messageContainer.nativeElement;
    setTimeout(() => {
      element.scrollTop = element.scrollHeight;
    }, 100);
  }

  toggleChatDetail(){
    this.isVisible = !this.isVisible;
  }

  toggleVideoCall(){
    this.isVideoCallVisible = !this.isVideoCallVisible;
  }
    


  sendMessage() {
    if (this.newMessage.trim() || this.audioUrl || this.selectedFiles.length > 0) {
      const formData = new FormData();
      formData.append('content', this.newMessage);
      formData.append('recipientId', this.selectedUser.id);
      formData.append('senderId', this.currentUser.id);
  
      // Gửi các file ảnh hoặc video kèm theo
      this.selectedFiles.forEach((file) => {
        formData.append('files', file.file);
      });
  
      // Gửi file audio nếu có
      if (this.audioUrl) {
        fetch(this.audioUrl)
          .then(res => res.blob())
          .then(blob => {
            formData.append('files', blob, 'recording.wav');
  
            this.messageService.addMessage(formData).subscribe({
              next: (response) => {
                this.messages.push(response as Message);
                this.signalRService.sendMessage((response as Message).content, this.selectedUser.id);

                this.scrollToBottom();
                this.resetMessageInput();

              },
              error: (error) => {
                console.log(error);
              }
            });
          })
          .catch(error => console.error('Error fetching audio:', error));
      } else {
        this.messageService.addMessage(formData).subscribe({
          next: (response) => {
            this.messages.push(response as Message);
            this.signalRService.sendMessage((response as Message).content, this.selectedUser.id);

            this.scrollToBottom();
            this.resetMessageInput();
          },
          error: (error) => {
            console.log(error);
          }
        });
      }
    }
  }
  


  getMessagesThread(){
    if(this.loading) return;
    this.loading = true;
    this.messageService.getMessagesThread(this.messageParams, this.currentUser.id, this.selectedUser.id).subscribe({
      next: (response) => {
        this.messages = response.result as Message[];
        this.pagination = response.pagination as Pagination;
        console.log(this.messages)
        this.loading = false;
      },
      error: (error) => {
        console.log(error);
      }
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


  getMessagesChatRoom(){
    this.messageService.getMessagesChatRoom(this.messageParams, this.selectedChatRoom.id).subscribe({
      next: (response) => {
        this.messages = response.result as Message[];
        this.pagination = response.pagination as Pagination;

        this.messages.forEach(message => {
          this.authService.getUserById(message.senderId).subscribe({
            next: (user) => {
              this.avatars[message.senderId] = user.profilePictureUrl;
            },
            error: (error) => {
              console.log(error);
            }
          });
        })


      },
      error: (error) => {
        console.log(error);
      }
    });
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

}
