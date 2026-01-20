import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Employee } from '../../../models/user-profile.model';

@Component({
  selector: 'app-add-employee-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-employee-modal.component.html',
  styleUrls: ['./add-employee-modal.component.css']
})
export class AddEmployeeModalComponent {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<Partial<Employee>>();

  formData = {
    employeeId: '',
    name: '',
    email: '',
    points: 0
  };

  onClose(): void {
    this.resetForm();
    this.close.emit();
  }

  onSave(): void {
    if (!this.formData.employeeId || !this.formData.name || !this.formData.email) {
      alert('Please fill all required fields');
      return;
    }

    if (!this.isValidEmail(this.formData.email)) {
      alert('Please enter a valid email address');
      return;
    }

    this.save.emit({ ...this.formData });
    this.resetForm();
  }

  resetForm(): void {
    this.formData = {
      employeeId: '',
      name: '',
      email: '',
      points: 0
    };
  }

  isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
}