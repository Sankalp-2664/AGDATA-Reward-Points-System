
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RedeemPoint } from '../../../models/redeem-point.model';

@Component({
  selector: 'app-update-redeem-point-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './update-redeem-point-modal.component.html',
  styleUrls: ['./update-redeem-point-modal.component.css']
})
export class UpdateRedeemPointModalComponent {
  @Input() isOpen = false;
  @Input() redeemPoint: RedeemPoint | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<number>();

  newValue: number = 0;

  ngOnChanges(): void {
    if (this.redeemPoint) {
      this.newValue = this.redeemPoint.value;
    }
  }

  onClose(): void {
    this.newValue = 0;
    this.close.emit();
  }

  onSave(): void {
    if (this.newValue <= 0) {
      alert('Redeem point value must be greater than zero');
      return;
    }
    this.save.emit(this.newValue);
    this.newValue = 0;
  }
}