import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Product, ProductFilter } from '../../models/product.model';
import { ProductStatus } from '../../models/enums';
import { SearchFilterComponent } from '../../shared/components/search-filter.component';
import { StatusFilterComponent } from '../../shared/components/status-filter.component';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule, SearchFilterComponent, StatusFilterComponent],
  templateUrl: './admin-products.component.html',
  styleUrls: ['./admin-products.component.css']
})
export class AdminProductsComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  
  filter: ProductFilter = {
    status: 'All',
    searchQuery: '',
    inStock: undefined
  };
  
  statusOptions: string[] = ['All', ProductStatus.Active, ProductStatus.Inactive, ProductStatus.OutOfStock];
  selectedStatus: string = 'All';
  searchQuery: string = '';
  
  showModal: boolean = false;
  modalMode: 'create' | 'edit' = 'create';
  selectedProduct: Product | null = null;
  
  productForm: Partial<Product> = this.getEmptyProductForm();
  
  categories: string[] = ['Electronics', 'Gift Cards', 'Office Supplies', 'Accessories', 'Other'];

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.products$.subscribe(products => {
      this.products = products;
      this.applyFilters();
    });
  }

  onSearch(query: string): void {
    this.filter.searchQuery = query;
    this.applyFilters();
  }

  onStatusChange(status: string): void {
    this.filter.status = status === 'All' ? 'All' : status as ProductStatus;
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = [...this.products];

    if (this.filter.status && this.filter.status !== 'All') {
      filtered = filtered.filter(product => product.status === this.filter.status);
    }

    if (this.filter.searchQuery && this.filter.searchQuery.trim()) {
      const query = this.filter.searchQuery.toLowerCase();
      filtered = filtered.filter(product =>
        product.name.toLowerCase().includes(query) ||
        product.productId.toLowerCase().includes(query) ||
        (product.description && product.description.toLowerCase().includes(query))
      );
    }

    if (this.filter.category) {
      filtered = filtered.filter(product => product.category === this.filter.category);
    }

    this.filteredProducts = filtered;
  }

  openCreateModal(): void {
    this.modalMode = 'create';
    this.productForm = this.getEmptyProductForm();
    this.selectedProduct = null;
    this.showModal = true;
  }

  openEditModal(product: Product): void {
    this.modalMode = 'edit';
    this.selectedProduct = product;
    this.productForm = { ...product };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.productForm = this.getEmptyProductForm();
    this.selectedProduct = null;
  }

  saveProduct(): void {
    if (this.modalMode === 'create') {
      console.log('Creating product:', this.productForm);
      alert('Product created successfully!');
    } else {
      console.log('Updating product:', this.productForm);
      alert('Product updated successfully!');
    }
    this.closeModal();
  }

  deleteProduct(product: Product): void {
    if (confirm(`Are you sure you want to delete product "${product.name}"?`)) {
      console.log('Deleting product:', product);
      alert('Product deleted successfully!');
    }
  }

  updateStock(product: Product): void {
    const newStock = prompt(`Update stock for "${product.name}" (Current: ${product.stock}):`, product.stock.toString());
    if (newStock !== null) {
      console.log('Updating stock:', { product, newStock });
      alert('Stock updated successfully!');
    }
  }

  getProductStatusClass(status: ProductStatus): string {
    switch (status) {
      case ProductStatus.Active:
        return 'badge-active';
      case ProductStatus.Inactive:
        return 'badge-inactive';
      case ProductStatus.OutOfStock:
        return 'badge-outofstock';
      default:
        return '';
    }
  }

  private getEmptyProductForm(): Partial<Product> {
    return {
      productId: '',
      name: '',
      description: '',
      stock: 0,
      points: 0,
      status: ProductStatus.Active,
      category: '',
      imageUrl: ''
    };
  }

  clearFilters(): void {
    this.filter = {
      status: 'All',
      searchQuery: '',
      inStock: undefined
    };
    this.selectedStatus = 'All';
    this.searchQuery = '';
    this.applyFilters();
  }
}
