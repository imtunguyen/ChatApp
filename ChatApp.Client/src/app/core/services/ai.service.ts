import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../shared/services/api.service";
import { Observable } from "rxjs";
import { AIResponse } from "../models/message.module";

@Injectable({
    providedIn: 'root',
  })
  export class AiService {
     private api = inject(ApiService);
  
    constructor() {}
  
    askAI(message: string): Observable<AIResponse> {
      return this.api.post<AIResponse>('message/ask-ai',  {message} );
    }
  }
  