import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface DateRangeFilter {
  startDate: string;
  endDate: string;
}

@Component({
  selector: 'app-date-range-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="date-range-container">
      <div class="date-input-group">
        <label class="date-label">From</label>
        <input
          type="date"
          [(ngModel)]="startDate"
          (ngModelChange)="onDateChange()"
          class="date-input"
          [max]="endDate || today"
        />
      </div>
      <div class="date-input-group">
        <label class="date-label">To</label>
        <input
          type="date"
          [(ngModel)]="endDate"
          (ngModelChange)="onDateChange()"
          class="date-input"
          [min]="startDate"
          [max]="today"
        />
      </div>
      <button *ngIf="startDate || endDate" (click)="clearDates()" class="clear-btn" type="button">
        Clear
      </button>
    </div>
  `,
  styles: [`
    .date-range-container {
      display: flex;
      align-items: flex-end;
      gap: 1rem;
      flex-wrap: wrap;
      margin-bottom: 1.5rem;
    }

    .date-input-group {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .date-label {
      font-size: 0.875rem;
      font-weight: 500;
      color: #374151;
    }

    .date-input {
      padding: 0.625rem 0.75rem;
      border: 1px solid #e5e7eb;
      border-radius: 0.5rem;
      font-size: 0.875rem;
      outline: none;
      transition: all 0.2s;
      min-width: 150px;
    }

    .date-input:focus {
      border-color: #3b82f6;
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
    }

    .clear-btn {
      padding: 0.625rem 1rem;
      background-color: #f3f4f6;
      color: #374151;
      border: 1px solid #e5e7eb;
      border-radius: 0.5rem;
      font-size: 0.875rem;
      cursor: pointer;
      transition: all 0.2s;
      font-weight: 500;
    }

    .clear-btn:hover {
      background-color: #e5e7eb;
    }
  `]
})
export class DateRangeFilterComponent {
  @Input() startDate: string = '';
  @Input() endDate: string = '';
  @Output() startDateChange = new EventEmitter<string>();
  @Output() endDateChange = new EventEmitter<string>();
  @Output() dateRangeChange = new EventEmitter<DateRangeFilter>();

  today: string = new Date().toISOString().split('T')[0];

  onDateChange(): void {
    this.startDateChange.emit(this.startDate);
    this.endDateChange.emit(this.endDate);
    this.dateRangeChange.emit({
      startDate: this.startDate,
      endDate: this.endDate
    });
  }

  clearDates(): void {
    this.startDate = '';
    this.endDate = '';
    this.onDateChange();
  }
}
