import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserService, Activity } from '../../services/user.service';

interface TransactionFilter {
  type?: 'all' | 'earned' | 'redeemed' | 'pending' | 'rejected';
  startDate?: string;
  endDate?: string;
  searchQuery?: string;
}

@Component({
  selector: 'app-my-rewards',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-rewards.component.html',
  styleUrls: ['./my-rewards.component.css']
})
export class MyRewardsComponent implements OnInit {
  activities: Activity[] = [];
  filteredActivities: Activity[] = [];
  
  // Filter properties
  filter: TransactionFilter = {
    type: 'all',
    searchQuery: '',
    startDate: '',
    endDate: ''
  };
  
  selectedFilterType: string = 'All';
  searchQuery: string = '';
  
  // Statistics
  totalEarned: number = 0;
  totalRedeemed: number = 0;
  totalPending: number = 0;
  totalRejected: number = 0;
  currentBalance: number = 0;

  // Expose Math to template
  Math = Math;

  constructor(
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadActivities();
    this.loadUserBalance();
  }

  loadActivities(): void {
    this.userService.activities$.subscribe(activities => {
      this.activities = activities;
      this.applyFilters();
      this.calculateTotals();
    });
  }

  loadUserBalance(): void {
    this.userService.currentUser$.subscribe(user => {
      this.currentBalance = user.points;
    });
  }

  calculateTotals(): void {
    this.totalEarned = this.activities
      .filter(a => a.type === 'earned')
      .reduce((sum, a) => sum + a.points, 0);

    this.totalRedeemed = Math.abs(
      this.activities
        .filter(a => a.type === 'redeemed')
        .reduce((sum, a) => sum + a.points, 0)
    );

    this.totalPending = this.activities
      .filter(a => a.type === 'pending')
      .length;

    this.totalRejected = this.activities
      .filter(a => a.type === 'rejected')
      .length;
  }

  onFilterTypeChange(type: string): void {
    this.filter.type = type.toLowerCase() as 'all' | 'earned' | 'redeemed' | 'pending' | 'rejected';
    this.applyFilters();
  }

  selectFilterType(type: string): void {
    this.selectedFilterType = type;
    this.filter.type = type.toLowerCase() as 'all' | 'earned' | 'redeemed' | 'pending' | 'rejected';
    this.applyFilters();
  }

  onDateRangeChange(dateRange: any): void {
    this.filter.startDate = dateRange.startDate;
    this.filter.endDate = dateRange.endDate;
    this.applyFilters();
  }

  onSearch(query: string): void {
    this.filter.searchQuery = query;
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = [...this.activities];

    // Type filter
    if (this.filter.type && this.filter.type !== 'all') {
      filtered = filtered.filter(a => a.type === this.filter.type);
    }

    // Date range filter
    if (this.filter.startDate) {
      filtered = filtered.filter(a => a.date >= this.filter.startDate!);
    }
    if (this.filter.endDate) {
      filtered = filtered.filter(a => a.date <= this.filter.endDate!);
    }

    // Search filter
    if (this.filter.searchQuery && this.filter.searchQuery.trim()) {
      const query = this.filter.searchQuery.toLowerCase();
      filtered = filtered.filter(a =>
        a.description.toLowerCase().includes(query)
      );
    }

    // Sort by date (newest first)
    filtered.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());

    this.filteredActivities = filtered;
  }

  clearFilters(): void {
    this.filter = {
      type: 'all',
      searchQuery: '',
      startDate: '',
      endDate: ''
    };
    this.selectedFilterType = 'All';
    this.searchQuery = '';
    this.applyFilters();
  }

  getActivityIcon(type: 'earned' | 'redeemed' | 'pending' | 'rejected'): string {
    switch (type) {
      case 'earned': return '💰';
      case 'redeemed': return '🎁';
      case 'pending': return '⏳';
      case 'rejected': return '❌';
      default: return '📋';
    }
  }

  getActivityClass(type: 'earned' | 'redeemed' | 'pending' | 'rejected'): string {
    switch (type) {
      case 'earned': return 'activity-earned';
      case 'redeemed': return 'activity-redeemed';
      case 'pending': return 'activity-pending';
      case 'rejected': return 'activity-rejected';
      default: return '';
    }
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}