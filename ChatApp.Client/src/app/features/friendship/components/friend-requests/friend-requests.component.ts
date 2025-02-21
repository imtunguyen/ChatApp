import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FriendShipService } from '../../../../core/services/friendship.service';
import { AuthService } from '../../../../core/services/auth.service';
import { FriendShipStatus } from '../../../../core/models/enum/friendship-status';
import { User } from '../../../../core/models/user.module';
import { ToastrService } from '../../../../shared/services/toastr.service';

@Component({
  selector: 'app-friend-requests',
  imports: [CommonModule],
  templateUrl: './friend-requests.component.html',
  styleUrl: './friend-requests.component.scss'
})
export class FriendRequestsComponent implements OnInit {
  friendRequests: any[] = [];
  requestUser: any;
  currentUser: any; 
  friendShipStatus: { [userId: string]: FriendShipStatus } = {};

  private authService = inject(AuthService);
  private friendShipService = inject(FriendShipService);
  private toastService = inject(ToastrService);
  constructor() {
    this.currentUser = this.authService.getCurrentUser();
  }
  ngOnInit() {
    this.loadFriendRequests();
  }
  getUserById(userId: string) {
    this.authService.getUserById(userId).subscribe((user) => {
      this.requestUser = user;
      console.log('User loaded!', user);
    });
    
    
  }
  loadFriendRequests() {
    this.friendShipService.getPendingRequests(this.currentUser.id).subscribe((requests) => {
      console.log('Friend requests loaded!', requests);
      this.friendRequests = requests;
      this.getUserById(requests[0].requesterId);
    });
  }
  acceptRequest(requestId: string) {
    console.log('Friend request accepted!', requestId);
    this.friendShipService.updateFriendShip(requestId, this.currentUser.id, FriendShipStatus.Accepted).subscribe(
      (res) => {
        this.toastService.showSuccess('Đã chấp nhận lời mời kết bạn');
        this.loadFriendRequests();
      },
      (err) => {
        this.toastService.showError('Lỗi khi chấp nh lời mời kết bạn');
        this.loadFriendRequests();
      }
    );
            
          
          
  }
  rejectRequest(requestId: string) {
    console.log('Friend request rejected!');
  }
}
