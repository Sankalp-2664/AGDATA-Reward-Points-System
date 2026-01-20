import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Event } from '../../../models/event.model';

@Component({
  selector: 'app-add-event-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-event-modal.component.html',
  styleUrls: ['./add-event-modal.component.css']
})
export class AddEventModalComponent {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<Partial<Event>>();

  formData = {
    eventId: '',
    name: '',
    description: '',
    firstPrize: 0,
    secondPrize: 0,
    thirdPrize: 0,
    startDate: '',
    endDate: ''
  };

  onClose(): void {
    this.resetForm();
    this.close.emit();
  }

  onSave(): void {
    if (!this.formData.eventId || !this.formData.name || !this.formData.description ||
        !this.formData.startDate || !this.formData.endDate) {
      alert('Please fill all required fields');
      return;
    }

    if (new Date(this.formData.endDate) < new Date(this.formData.startDate)) {
      alert('End date must be after start date');
      return;
    }

    if (this.formData.firstPrize < 0 || this.formData.secondPrize < 0 || this.formData.thirdPrize < 0) {
      alert('Prize points cannot be negative');
      return;
    }

    this.save.emit({ ...this.formData });
    this.resetForm();
  }

  resetForm(): void {
    this.formData = {
      eventId: '',
      name: '',
      description: '',
      firstPrize: 0,
      secondPrize: 0,
      thirdPrize: 0,
      startDate: '',
      endDate: ''
    };
  }
}