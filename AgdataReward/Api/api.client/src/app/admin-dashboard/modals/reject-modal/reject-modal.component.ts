import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PendingRequest } from '../../../models/pending-request.model';

@Component({
  selector: 'app-reject-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reject-modal.component.html',
  styleUrls: ['./reject-modal.component.css']
})
export class RejectModalComponent {
  @Input() isOpen = false;
  @Input() request: PendingRequest | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() reject = new EventEmitter<string>();

  rejectReason = '';

  onClose(): void {
    this.rejectReason = '';
    this.close.emit();
  }

  onReject(): void {
    if (!this.rejectReason.trim()) {
      alert('Please provide a reason for rejection');
      return;
    }
    this.reject.emit(this.rejectReason);
    this.rejectReason = '';
  }
}