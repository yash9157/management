import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface NotificationMessage { text: string; type: 'success' | 'danger'; }

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly messageSubject = new BehaviorSubject<NotificationMessage | null>(null);
  readonly message$ = this.messageSubject.asObservable();
  success(text: string): void { this.show(text, 'success'); }
  error(text: string): void { this.show(text, 'danger'); }
  clear(): void { this.messageSubject.next(null); }
  private show(text: string, type: 'success' | 'danger'): void {
    this.messageSubject.next({ text, type });
    window.setTimeout(() => this.clear(), 4000);
  }
}
