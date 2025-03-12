import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../shared/services/api.service';
import { Group } from '../models/group.module';

@Injectable({
  providedIn: 'root'
})
export class GroupService {

  private api = inject(ApiService);
  constructor() { }

  addGroup(group: FormData){
    return this.api.post<Group>('group/add', group);
  }
  getGroupsByUser(userId: string){
    return this.api.get<Group[]>('group/get?userId='+ userId);
  }

  getUsersByGroup(groupId: number){
    return this.api.get<Group>('usergroup/get?groupId='+ groupId);
  }
}
