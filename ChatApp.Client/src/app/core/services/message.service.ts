import { Injectable } from "@angular/core";

import { BehaviorSubject, map, Observable, Subject } from "rxjs";
import { send } from "process";
import { environment } from "../../../environments/environment";
import { Message } from "../models/message.module";
import { HttpClient, HttpParams, HttpResponse } from "@angular/common/http";
import { MessageParams } from "../../shared/params/messageParams";
import { PaginatedResult } from "../../shared/models/pagination.module";

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private apiUrl = `${environment.apiUrl}message`;
  private paginatedResult: PaginatedResult<Message[]> = {  };

  private messageUpdateSubject = new Subject<void>();
  messageUpdate = this.messageUpdateSubject.asObservable();
  constructor(private http: HttpClient) { }

  //add message
  addMessage(message: any) {
    return this.http.post(`${this.apiUrl}/add`, message);
  }

  //updateMessage\
  updateMessage(message: any) {
    return this.http.put(`${this.apiUrl}/update`, message);
  }

  deleteMessage(id: number) {
    return this.http.put(`${this.apiUrl}/delete?id=${id}`, null);
  }

  getLastMessage(senderId: string, recipientId: string) {
    return this.http.get<Message>(`${this.apiUrl}/GetLastMessage?senderId=${senderId}&recipientId=${recipientId}`);
  }

  getLastMessageGroup(groupId: number) {
    return this.http.get<Message>(`${this.apiUrl}/GetLastMessageGroup?groupId=${groupId}`);
  }

  getMessagesThread(params: any, senderId: string, recipientId: string): Observable<PaginatedResult<Message[]>> {
    let httpParams = new HttpParams();
    if (params.pageNumber) {
      httpParams = httpParams.set('PageNumber', params.pageNumber.toString());
    }
    if (params.pageSize) {
      httpParams = httpParams.set('PageSize', params.pageSize.toString());
    }
    if (params.search) {
      httpParams = httpParams.set('Search', params.search);
    }
    return this.http.get<Message[]>(`${this.apiUrl}/GetMessagesThread?senderId=${senderId}&recipientId=${recipientId}`, {
      params: httpParams,
      observe: 'response'
    }).pipe(
      map((response: HttpResponse<Message[]>) => {
        const paginatedResult = new PaginatedResult<Message[]>();
        paginatedResult.items = response.body || [];

        const paginationHeader = response.headers.get('Pagination');
        if (paginationHeader) {
          paginatedResult.pagination = JSON.parse(paginationHeader);
        }

        return paginatedResult;
      })
    );
  }

  getMessagesGroup(params: any, groupId: number): Observable<PaginatedResult<Message[]>> {
    let httpParams = new HttpParams();
    if (params.pageNumber) {
      httpParams = httpParams.set('PageNumber', params.pageNumber.toString());
    }
    if (params.pageSize) {
      httpParams = httpParams.set('PageSize', params.pageSize.toString());
    }
    if (params.search) {
      httpParams = httpParams.set('Search', params.search);
    }
    return this.http.get<Message[]>(
      `${this.apiUrl}/GetMessagesgroup?groupId=${groupId}`,
      { params: httpParams,
        observe: 'response' }
    ).pipe(
      map((response: HttpResponse<Message[]>) => {
        const paginatedResult = new PaginatedResult<Message[]>();
        paginatedResult.items = response.body || [];

        const paginationHeader = response.headers.get('Pagination');
        if (paginationHeader) {
          paginatedResult.pagination = JSON.parse(paginationHeader);
        }

        return paginatedResult;
      })
    );
  }

  
  notifyMessageUpdate(){
    this.messageUpdateSubject.next();
  }

  markAsRead(messageId: number) {
    return this.http.put(`${this.apiUrl}/markAsRead?messageId=${messageId}`, null);
  }
  

}
