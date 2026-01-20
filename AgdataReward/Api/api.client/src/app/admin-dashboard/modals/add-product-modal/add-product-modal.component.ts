import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Product {
    productId: string;
    name: string;
    stock: number;
    points: number;
    status: 'Active' | 'Inactive';
}

@Component({
    selector: 'app-add-product-modal',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './add-product-modal.component.html',
    styleUrls: ['./add-product-modal.component.css']
})
export class AddProductModalComponent {
    @Input() isOpen = false;
    @Output() close = new EventEmitter<void>();
    @Output() save = new EventEmitter<Partial<Product>>();

    formData = {
        productId: '',
        name: '',
        stock: 0,
        points: 0,
        status: 'Active' as 'Active' | 'Inactive'
    };

    onClose(): void {
        this.resetForm();
        this.close.emit();
    }

    onSave(): void {
        if (!this.formData.productId || !this.formData.name) {
            alert('Please fill all required fields');
            return;
        }

        if (this.formData.stock < 0 || this.formData.points < 0) {
            alert('Stock and points cannot be negative');
            return;
        }

        this.save.emit({ ...this.formData });
        this.resetForm();
    }

    resetForm(): void {
        this.formData = {
            productId: '',
            name: '',
            stock: 0,
            points: 0,
            status: 'Active'
        };
    }
}
