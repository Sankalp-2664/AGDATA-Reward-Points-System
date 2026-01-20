import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

type BadgeVariant = 'success' | 'error' | 'warning' | 'info';

/**
 * Reusable Badge Component
 * Used for status indicators and labels
 */
@Component({
  selector: 'ag-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [class]="badgeClass">
      <ng-content></ng-content>
    </span>
  `,
  styles: [`
    :host {
      display: inline-block;
    }

    span {
      font: var(--ag-label);
      padding: 4px 8px;
      border-radius: var(--ag-radius-full);
      display: inline-block;
      white-space: nowrap;
    }

    /* Success Badge (Green) */
    span.success {
      background-color: var(--ag-color-success-lightest);
      color: var(--ag-color-success);
      border: 1px solid var(--ag-color-success-light);
    }

    /* Error Badge (Red) */
    span.error {
      background-color: var(--ag-color-error-lightest);
      color: var(--ag-color-error);
      border: 1px solid var(--ag-color-error-light);
    }

    /* Warning Badge (Orange) */
    span.warning {
      background-color: var(--ag-color-warning-lightest);
      color: var(--ag-color-warning);
      border: 1px solid var(--ag-color-warning-light);
    }

    /* Info Badge */
    span.info {
      background-color: var(--ag-color-gray-100);
      color: var(--ag-color-gray-600);
      border: 1px solid var(--ag-color-gray-200);
    }
  `]
})
export class AgBadgeComponent {
  @Input() variant: BadgeVariant = 'info';

  get badgeClass(): string {
    return this.variant;
  }
}
