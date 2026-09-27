import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  template: `
    @if (totalPages > 1) {
      <nav aria-label="Employee pages">
        <ul class="pagination mb-0">
          <li class="page-item" [class.disabled]="page === 1">
            <button class="page-link" type="button" (click)="changePage(page - 1)">Previous</button>
          </li>
          @for (pageNumber of visiblePages; track pageNumber) {
            <li class="page-item" [class.active]="pageNumber === page">
              <button class="page-link" type="button" (click)="changePage(pageNumber)">{{ pageNumber }}</button>
            </li>
          }
          <li class="page-item" [class.disabled]="page === totalPages">
            <button class="page-link" type="button" (click)="changePage(page + 1)">Next</button>
          </li>
        </ul>
      </nav>
    }
  `
})
export class PaginationComponent {
  @Input() page = 1;
  @Input() totalPages = 0;
  @Output() pageChange = new EventEmitter<number>();

  get visiblePages(): number[] {
    const start = Math.max(1, Math.min(this.page - 2, this.totalPages - 4));
    const end = Math.min(this.totalPages, start + 4);
    return Array.from({ length: end - start + 1 }, (_, index) => start + index);
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.page) this.pageChange.emit(page);
  }
}
