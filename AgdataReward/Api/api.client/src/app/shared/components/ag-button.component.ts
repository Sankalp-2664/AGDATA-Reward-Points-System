import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

type ButtonVariant = 'primary' | 'secondary' | 'outline' | 'danger';
type ButtonSize = 'sm' | 'md' | 'lg';

/**
 * Reusable Button Component
 * Supports multiple variants and sizes
 */
@Component({
  selector: 'ag-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      [type]="type"
      [disabled]="disabled"
      [class]="buttonClass"
      (click)="click.emit()"
    >
      <ng-content></ng-content>
    </button>
  `,
  styles: [`
    :host {
      display: inline-block;
    }

    button {
      font: var(--ag-button-text);
      padding: var(--ag-spacing-sm) var(--ag-spacing-md);
      border: none;
      border-radius: var(--ag-radius-md);
      cursor: pointer;
      transition: all var(--ag-transition-base);
      font-weight: 500;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: var(--ag-spacing-sm);
      white-space: nowrap;
      text-decoration: none;
    }

    /* Sizes */
    button.sm {
      padding: var(--ag-spacing-xs) var(--ag-spacing-sm);
      font-size: 12px;
    }

    button.lg {
      padding: var(--ag-spacing-md) var(--ag-spacing-lg);
      font-size: 16px;
    }

    /* Primary Button */
    button.primary {
      background-color: var(--ag-color-primary);
      color: white;
      box-shadow: var(--ag-shadow-sm);
    }

    button.primary:hover:not(:disabled) {
      background-color: var(--ag-color-primary-dark);
      box-shadow: var(--ag-shadow-md);
      transform: translateY(-1px);
    }

    button.primary:active:not(:disabled) {
      transform: translateY(0);
      box-shadow: var(--ag-shadow-xs);
    }

    /* Secondary Button */
    button.secondary {
      background-color: var(--ag-color-gray-100);
      color: var(--ag-color-gray-dark);
    }

    button.secondary:hover:not(:disabled) {
      background-color: var(--ag-color-gray-200);
    }

    /* Outline Button */
    button.outline {
      border: 2px solid var(--ag-color-primary);
      background-color: transparent;
      color: var(--ag-color-primary);
    }

    button.outline:hover:not(:disabled) {
      background-color: var(--ag-color-primary-lightest);
    }

    /* Danger Button */
    button.danger {
      background-color: var(--ag-color-error);
      color: white;
    }

    button.danger:hover:not(:disabled) {
      background-color: #c62828;
    }

    /* Disabled State */
    button:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  `]
})
export class AgButtonComponent {
  @Input() variant: ButtonVariant = 'primary';
  @Input() size: ButtonSize = 'md';
  @Input() disabled = false;
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Output() click = new EventEmitter<void>();

  get buttonClass(): string {
    return `${this.variant} ${this.size}`;
  }
}
