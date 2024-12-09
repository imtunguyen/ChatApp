import { Injectable } from '@angular/core';
import { TuiAlertService } from '@taiga-ui/core';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private readonly alerts: TuiAlertService) {}

  showSuccess(message: string): void {
    this.alerts.open(message, {
      label: 'Success',
      appearance: 'positive',
      autoClose: 2000,
      closeable: true,
      icon: '@tui.check',
    }).subscribe();
  }

  showError(message: string): void {
    this.alerts.open(message, {
      label: 'Error',
      appearance: 'negative',
      autoClose: 2000,
      closeable: true,
      icon: '@tui.circle-x',
    }).subscribe();
  }

  showWarning(message: string): void {
    this.alerts.open(message, {
      label: 'Warning',
      appearance: 'warning',
      autoClose: 2000,
      closeable: true,
      icon: '@tui.triangle-alert',
    }).subscribe();
  }

  showInfo(message: string): void {
    this.alerts.open(message, {
      label: 'Info',
      appearance: 'info',
      autoClose: 2000,
      closeable: true,
      icon: '@tui.info',
    }).subscribe();
  }
}
