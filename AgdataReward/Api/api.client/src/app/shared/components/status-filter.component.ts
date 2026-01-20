import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-status-filter',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="status-filter-container">
      <div class="filter-label">Filter by Status:</div>
      <div class="status-buttons">
        <button
          *ngFor="let status of statuses"
          (click)="selectStatus(status)"
          [class.active]="selectedStatus === status"
          class="status-btn"
          type="button"
        >
          {{ status }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .status-filter-container {
      margin-bottom: 1.5rem;
    }

    .filter-label {
      font-size: 0.875rem;
      font-weight: 600;
      color: #374151;
      margin-bottom: 0.75rem;
    }

    .status-buttons {
      display: flex;
      gap: 0.5rem;
      flex-wrap: wrap;
    }

    .status-btn {
      padding: 0.5rem 1rem;
      background-color: #f3f4f6;
      color: #6b7280;
      border: 1px solid #e5e7eb;
      border-radius: 0.5rem;
      font-size: 0.875rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }

    .status-btn:hover {
      background-color: #e5e7eb;
    }

    .status-btn.active {
      background-color: #3b82f6;
      color: white;
      border-color: #3b82f6;
    }
  `]
})
export class StatusFilterComponent {
  @Input() statuses: string[] = ['All'];
  @Input() selectedStatus: string = 'All';
  @Output() selectedStatusChange = new EventEmitter<string>();
  @Output() statusChange = new EventEmitter<string>();

  selectStatus(status: string): void {
    this.selectedStatus = status;
    this.selectedStatusChange.emit(status);
    this.statusChange.emit(status);
  }
}
