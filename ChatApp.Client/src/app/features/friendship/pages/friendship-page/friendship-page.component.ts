import { Component } from '@angular/core';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { FriendsListComponent } from '../../components/friends-list/friends-list.component';
import { GroupsListComponent } from '../../components/groups-list/groups-list.component';
import { FriendRequestsComponent } from '../../components/friend-requests/friend-requests.component';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-friendship-page',
  imports: [NzIconModule, FriendsListComponent, GroupsListComponent, FriendRequestsComponent, CommonModule],
  templateUrl: './friendship-page.component.html',
  styleUrl: './friendship-page.component.scss'
})
export class FriendshipPageComponent {
  selectedTab: string = 'friends';

  selectTab(tab: string) {
    this.selectedTab = tab;
  }
}
