import { Component, inject } from '@angular/core';
import { User } from '../../../../core/models/user.module';
import { AuthService } from '../../../../core/services/auth.service';
import { FriendShipService } from '../../../../core/services/friendship.service';
import { ToastrService } from '../../../../shared/services/toastr.service';
import { FriendShip } from '../../../../core/models/friendship.module';
import { CommonModule } from '@angular/common';
import { FriendShipStatus } from '../../../../core/models/enum/friendship-status';

@Component({
  selector: 'app-friends-list',
  imports: [CommonModule],
  templateUrl: './friends-list.component.html',
  styleUrl: './friends-list.component.scss'
})
export class FriendsListComponent {
  currentUser: any; 
  friendsList: User[] = [];
  
  private authService = inject(AuthService);
  private friendShipService = inject(FriendShipService);
  private toastService = inject(ToastrService);
  constructor() {
    this.currentUser = this.authService.getCurrentUser();
  }
  

  ngOnInit() {
    this.loadFriends();
  }
  
  loadFriends() {
    this.friendShipService.getFriends(this.currentUser.id).subscribe(
      (res: FriendShip[]) => {
        res.forEach(friendship => {
          if (friendship.requesterId === this.currentUser.id) {
            this.authService.getUserById(friendship.addresseeId).subscribe(
              (user: User) => {
                this.friendsList.push(user);
              }
            );
          } else {
            this.authService.getUserById(friendship.requesterId).subscribe(
              (user: User) => {
                this.friendsList.push(user);
              }
            );
          }
        });
      }
    );
      
  }

  removeFriend(friendId: string) {
    console.log('Friend removed!');
    this.friendShipService.updateFriendShip(this.currentUser.id, friendId, FriendShipStatus.None).subscribe(
          (res) => {
            this.toastService.showSuccess('Đã hủy kết bạn');
            this.loadFriends();
          },
          (err) => {
            this.toastService.showError('Lỗi khi hủy kết bạn');
            this.loadFriends();
          }
        );
  }
  

}
