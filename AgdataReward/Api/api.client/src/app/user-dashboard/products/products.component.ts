import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { RedemptionService } from '../../services/redemption.service';
import { Product, ProductFilter } from '../../models/product.model';
import { ProductStatus } from '../../models/enums';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css'],
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  searchQuery: string = '';
  userPoints: number = 0;
  userId: string = '';
  filterDropdownOpen: boolean = false;
  selectedRange: string = 'All';

  // Filter properties
  filter: ProductFilter = {
    status: 'All',
    searchQuery: '',
    inStock: true,
    minPoints: undefined,
    maxPoints: undefined
  };

  // Point ranges for dropdown
  pointRanges = [
    { label: 'All', min: 0, max: 999999 },
    { label: '1-200 pts', min: 1, max: 200 },
    { label: '201-500 pts', min: 201, max: 500 },
    { label: '501-1000 pts', min: 501, max: 1000 },
    { label: '1001-10000 pts', min: 1001, max: 10000 }
  ];

  constructor(
    private productService: ProductService,
    private redemptionService: RedemptionService,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadUserPoints();
  }

  loadProducts(): void {
    this.productService.products$.subscribe((products) => {
      this.products = products;
      this.applyFilters();
    });
  }

  loadUserPoints(): void {
    this.userService.currentUser$.subscribe((user) => {
      this.userPoints = user.points;
      // Try to get from localStorage first (set during login), otherwise use mock id
      const storedUserId = localStorage.getItem('userId');
      this.userId = storedUserId || user.id.toString();
    });
  }

  onSearch(query: string): void {
    this.filter.searchQuery = query;
    this.applyFilters();
  }

  toggleFilterDropdown(): void {
    this.filterDropdownOpen = !this.filterDropdownOpen;
  }

  selectPointRange(range: { label: string; min: number; max: number }): void {
    this.selectedRange = range.label;
    this.filter.minPoints = range.min;
    this.filter.maxPoints = range.max === 999999 ? undefined : range.max;
    this.filterDropdownOpen = false;
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = [...this.products];

    // Search filter
    if (this.filter.searchQuery && this.filter.searchQuery.trim()) {
      const query = this.filter.searchQuery.toLowerCase();
      filtered = filtered.filter(product =>
        product.name.toLowerCase().includes(query) ||
        product.productId.toLowerCase().includes(query) ||
        (product.description && product.description.toLowerCase().includes(query))
      );
    }

    // Points range filter
    if (this.filter.minPoints !== undefined) {
      filtered = filtered.filter(product => product.points >= this.filter.minPoints!);
    }
    if (this.filter.maxPoints !== undefined) {
      filtered = filtered.filter(product => product.points <= this.filter.maxPoints!);
    }

    // Filter out inactive products (only show Active products to users)
    filtered = filtered.filter(product => product.status === ProductStatus.Active);

    // Filter out out-of-stock products
    filtered = filtered.filter(product => product.stock > 0);

    this.filteredProducts = filtered;
  }

  canRedeem(product: Product): boolean {
    return this.userPoints >= product.points && product.stock > 0;
  }

  redeemProduct(product: Product): void {
    if (!this.canRedeem(product)) {
      alert('Insufficient points or product out of stock');
      return;
    }

    // First check if user already has a pending request for this product
    this.redemptionService.hasPendingRequestForProduct(this.userId, product.id).subscribe({
      next: (hasPending) => {
        if (hasPending) {
          alert(`You already have a pending request for ${product.name}. Please wait for admin approval.`);
          return;
        }

        // Show confirmation dialog
        const confirmMessage = `Do you want to redeem ${product.name} for ${product.points} points?\n\nYour points will be deducted immediately and a request will be sent to admin for approval.`;
        const confirmed = window.confirm(confirmMessage);

        if (confirmed) {
          // Call backend API to request redemption
          this.redemptionService.requestRedemption(this.userId, product.id).subscribe({
            next: (response) => {
              alert(`Redemption request submitted successfully for ${product.name}!\n\nYour points have been deducted. Please wait for admin approval.`);
              // Reload user points to reflect deduction
              this.loadUserPoints();
              // Reload products to update stock
              this.loadProducts();
            },
            error: (error) => {
              console.error('Error requesting redemption:', error);
              const errorMsg = error?.error?.message || 'Failed to process redemption request. Please try again.';
              alert(errorMsg);
            }
          });
        }
      },
      error: (error) => {
        console.error('Error checking pending requests:', error);
        alert('Failed to check existing requests. Please try again.');
      }
    });
  }

  clearFilters(): void {
    this.filter = {
      status: 'All',
      searchQuery: '',
      inStock: true,
      minPoints: undefined,
      maxPoints: undefined
    };
    this.selectedRange = 'All';
    this.searchQuery = '';
    this.applyFilters();
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}
