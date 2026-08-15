import { Injectable } from '@angular/core';

export interface ToastMessage {
  type: 'success' | 'error' | 'warning';
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  messages: ToastMessage[] = [];

  success(message: string) {
    this.add({ type: 'success', message });
  }

  error(message: string) {
    this.add({ type: 'error', message });
  }

  warning(message: string) {
    this.add({ type: 'warning', message });
  }

  private add(toast: ToastMessage) {
    this.messages.push(toast);
    setTimeout(() => {
      this.messages.shift();
    }, 4000); // 4 seconds
  }
}
