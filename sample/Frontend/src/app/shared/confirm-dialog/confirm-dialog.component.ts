import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  template: `
    @if (open) {
      <div class="modal d-block" tabindex="-1" role="dialog" aria-modal="true">
        <div class="modal-dialog modal-dialog-centered">
          <div class="modal-content shadow">
            <div class="modal-header"><h2 class="modal-title fs-5">{{ title }}</h2></div>
            <div class="modal-body"><p class="mb-0">{{ message }}</p></div>
            <div class="modal-footer">
              <button class="btn btn-outline-secondary" type="button" (click)="cancelled.emit()">Cancel</button>
              <button class="btn btn-danger" type="button" (click)="confirmed.emit()">Delete</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop show"></div>
    }
  `
})
export class ConfirmDialogComponent {
  @Input() open = false;
  @Input() title = 'Confirm action';
  @Input() message = 'Are you sure?';
  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();
}
