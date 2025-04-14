import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../shared/services/api.service';
import { Group } from '../models/group.module';
import { UserGroup, UserGroupAdd } from '../models/user-group.module';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subject, Observable, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GroupService {

  private api = inject(ApiService);
  private authService = inject(AuthService);
  private apiUrl = environment.apiUrl;

  private groupUpdateSubject = new Subject<void>();
  groupUpdate = this.groupUpdateSubject.asObservable();
  constructor(private http: HttpClient) { }

  addGroup(group: FormData){
    return this.api.post<Group>('group/add', group);
  }
  getGroupsByUser(userId: string){
    return this.api.get<Group[]>('group/get?userId='+ userId);
  }

  getUsersByGroup(groupId: number){
    return this.api.get<Group>('usergroup/get?groupId='+ groupId);
  }

  addMemberToGroup(userGroups: UserGroupAdd[]){
    console.log("them user vao group", userGroups);
    return this.api.post<UserGroup>('usergroup/addmultiple', userGroups);
  }

  removeMember(userId: string, groupId: number): Observable<any> {
   
    return this.api.put<any>('usergroup/remove?userId='+ userId + '&groupId=' + groupId, null);
  }
  
  deleteGroup(groupId: number): Observable<any> {
    return this.api.delete<any>('group/delete?groupId='+ groupId);
  }
  
  updateRole(userGroup: UserGroup): Observable<any> {
    return this.api.put<any>('usergroup/updateRole', userGroup);
  }

  notifyGroupUpdate(){
    this.groupUpdateSubject.next();
  }
}
