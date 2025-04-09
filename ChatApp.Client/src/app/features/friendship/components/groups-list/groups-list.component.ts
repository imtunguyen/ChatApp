import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../../core/services/auth.service';
import { GroupService } from '../../../../core/services/group.service';
import { Group } from '../../../../core/models/group.module';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-groups-list',
  imports: [CommonModule],
  templateUrl: './groups-list.component.html',
  styleUrl: './groups-list.component.scss'
})
export class GroupsListComponent implements OnInit {
  currentUser: any;
  groupsList: Group[] = [];

  private groupService = inject(GroupService);
  private authService = inject(AuthService);
  constructor() {
    this.currentUser = this.authService.getCurrentUser();
  }
  ngOnInit(){
    this.loadGroups();
  }
  

  loadGroups() {
    this.groupService.getGroupsByUser(this.currentUser.id).subscribe((groups) => {
      console.log("Groups:", groups);
      this.groupsList = groups;
        console.log(this.groupsList);
      
    });
  }


}
