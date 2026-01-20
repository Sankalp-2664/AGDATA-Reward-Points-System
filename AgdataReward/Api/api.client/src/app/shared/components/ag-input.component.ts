import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

/**
 * Reusable Input Component with Label and Error Handling
 */
@Component({
  selector: 'ag-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="ag-form-group">
      <label *ngIf="label" class="ag-label" [for]="name">
        {{ label }}
        <span *ngIf="required" class="required">*</span>
      </label>
      <input
        [type]="type"
        [id]="name"
        [name]="name"
        [placeholder]="placeholder"
        [disabled]="disabled"
        [formControl]="control"
        [class.error]="hasError"
        class="ag-input-field"
      />
      <p *ngIf="hasError" class="ag-helper-text error-message">
        {{ errorMessage }}
      </p>
    </div>
  `,
  styles: [`
    .ag-form-group {
      margin-bottom: var(--ag-spacing-md);
      display: flex;
      flex-direction: column;
      gap: var(--ag-spacing-xs);
    }

    label {
      color: var(--ag-color-gray-dark);
      display: flex;
      gap: 4px;
    }

    .required {
      color: var(--ag-color-error);
    }

    .ag-input-field {
      padding: var(--ag-spacing-sm) var(--ag-spacing-md);
      border: 1px solid var(--ag-color-gray-200);
      border-radius: var(--ag-radius-md);
      font: var(--ag-body-01);
      color: var(--ag-color-gray-dark);
      background-color: var(--ag-color-white);
      transition: all var(--ag-transition-base);
    }

    .ag-input-field:focus {
      outline: none;
      border-color: var(--ag-color-primary);
      box-shadow: 0 0 0 3px rgba(46, 125, 50, 0.1);
    }

    .ag-input-field:disabled {
      background-color: var(--ag-color-gray-50);
      color: var(--ag-color-gray-400);
      cursor: not-allowed;
    }

    .ag-input-field.error {
      border-color: var(--ag-color-error);
      background-color: var(--ag-color-error-lightest);
    }

    .error-message {
      color: var(--ag-color-error);
      margin: 0;
    }
  `]
})
export class AgInputComponent {
  @Input() label?: string;
  @Input() type: string = 'text';
  @Input() name: string = '';
  @Input() placeholder: string = '';
  @Input() disabled = false;
  @Input() required = false;
  @Input() control: any;
  @Input() errorMessage: string = 'This field is required';

  get hasError(): boolean {
    return this.control && this.control.invalid && (this.control.dirty || this.control.touched);
  }
}
