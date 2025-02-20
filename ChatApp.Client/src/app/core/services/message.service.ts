import { Injectable } from "@angular/core";

import { map } from "rxjs";
import { send } from "process";
import { environment } from "../../../environments/environment";
import { PaginationResult } from "../../shared/models/pagination.module";
import { Message } from "../models/message.module";
import { HttpClient, HttpParams } from "@angular/common/http";
import { MessageParams } from "../../shared/params/messageParams";

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private apiUrl = `${environment.apiUrl}message`;
  private paginationResult: PaginationResult<Message[]> = {  };
  constructor(private http: HttpClient) { }

  //add message
  addMessage(message: any) {
    return this.http.post(`${this.apiUrl}/add`, message);
  }

  getMessagesThread(messageParams: MessageParams, senderId: string, recipientId: string) {
    let params = new HttpParams()
      params.append('pageNumber', messageParams.pageNumber.toString());
      params.append('pageSize', messageParams.pageSize.toString())
      params.append('orderBy', messageParams.orderBy);

    if (messageParams.search) {
      params = params.append('search', messageParams.search);
    }

    return this.http.get<PaginationResult<Message[]>>(
      `${this.apiUrl}/GetMessagesThread?senderId=${senderId}&recipientId=${recipientId}`,
      { observe: 'response', params }
    ).pipe(
      map(response => {
        // Kiểm tra và trả về dữ liệu
        this.paginationResult.result = response.body as Message[];
        const pagination = response.headers.get('Pagination');
          if (pagination !== null) {
            this.paginationResult.pagination = JSON.parse(pagination);
          }
          return this.paginationResult;
      })
    );
  }

  getMessagesChatRoom(messageParams: MessageParams, chatRoomId: number) {
    let params = new HttpParams()
      params.append('pageNumber', messageParams.pageNumber.toString());
      params.append('pageSize', messageParams.pageSize.toString())
      params.append('orderBy', messageParams.orderBy);

    if (messageParams.search) {
      params = params.append('search', messageParams.search);
    }

    return this.http.get<PaginationResult<Message[]>>(
      `${this.apiUrl}/GetMessagesChatRoom?chatRoomId=${chatRoomId}`,
      { observe: 'response', params }
    ).pipe(
      map(response => {
        // Kiểm tra và trả về dữ liệu
        this.paginationResult.result = response.body as Message[];
        const pagination = response.headers.get('Pagination');
          if (pagination !== null) {
            this.paginationResult.pagination = JSON.parse(pagination);
          }
          return this.paginationResult;
      })
    );
  }

}
