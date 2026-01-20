import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../models/product.model';

@Component({
  selector: 'app-update-stock-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './update-stock-modal.component.html',
  styleUrls: ['./update-stock-modal.component.css']
})
export class UpdateStockModalComponent {
  @Input() isOpen = false;
  @Input() product: Product | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<number>();

  newStock: number = 0;

  ngOnChanges(): void {
    if (this.product) {
      this.newStock = this.product.stock;
    }
  }

  onClose(): void {
    this.newStock = 0;
    this.close.emit();
  }

  onSave(): void {
    if (this.newStock < 0) {
      alert('Stock quantity cannot be negative');
      return;
    }
    this.save.emit(this.newStock);
    this.newStock = 0;
  }
}