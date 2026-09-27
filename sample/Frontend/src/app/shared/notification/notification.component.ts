import { AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-notification',
  imports: [AsyncPipe],
  template: `
    @if (notificationService.message$ | async; as message) {
      <div class="notification alert alert-{{ message.type }} alert-dismissible shadow" role="alert">
        {{ message.text }}
        <button type="button" class="btn-close" aria-label="Close" (click)="notificationService.clear()"></button>
      </div>
    }
  `,
  styles: `.notification { position: fixed; top: 4.5rem; right: 1rem; z-index: 1100; min-width: 20rem; max-width: calc(100vw - 2rem); }`
})
export class NotificationComponent {
  constructor(public notificationService: NotificationService) {}
}
