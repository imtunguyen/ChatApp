import { Injectable } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';

@Injectable({
  providedIn: 'root',
})
export class ToastrService {
  constructor(private message: NzMessageService) {}

  showInfo(message: string): void {
    this.message.info(message);
  }

  showSuccess(message: string): void {
    this.message.success(message);
  }

  showError(message: string): void {
    this.message.error(message);
  }

  showWarning(message: string): void {
    this.message.warning(message);
  }
}