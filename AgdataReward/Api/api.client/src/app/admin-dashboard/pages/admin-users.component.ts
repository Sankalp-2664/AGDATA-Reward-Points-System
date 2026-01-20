import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsersService } from '../../services/users.service';
import { UserProfileDto, UserFilter, CreateUserRequest } from '../../models/user-profile.model';
import { AccountStatus, UserRole } from '../../models/enums';
import { SearchFilterComponent } from '../../shared/components/search-filter.component';
import { StatusFilterComponent } from '../../shared/components/status-filter.component';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, FormsModule, SearchFilterComponent, StatusFilterComponent],
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css']
})
export class AdminUsersComponent implements OnInit {
  users: UserProfileDto[] = [];
  filteredUsers: UserProfileDto[] = [];
  
  filter: UserFilter = {
    status: 'All',
    role: 'All',
    searchQuery: ''
  };
  
  statusOptions: string[] = ['All', AccountStatus.Active, AccountStatus.Inactive, AccountStatus.Suspended];
  roleOptions: string[] = ['All', UserRole.User, UserRole.Admin];
  selectedStatus: string = 'All';
  selectedRole: string = 'All';
  searchQuery: string = '';
  
  showModal: boolean = false;
  showPointsModal: boolean = false;
  modalMode: 'create' | 'edit' = 'create';
  selectedUser: UserProfileDto | null = null;
  
  userForm: Partial<CreateUserRequest> = this.getEmptyUserForm();
  pointsAdjustment = {
    points: 0,
    reason: '',
    type: 'add' as 'add' | 'deduct'
  };

  constructor(private usersService: UsersService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    // Mock data for now - replace with actual service call
    this.users = [
      {
        id: '1',
        employeeId: 'AGD-001',
        firstName: 'John',
        lastName: 'Doe',
        email: 'john.doe@agdata.com',
        role: UserRole.User,
        department: 'Engineering',
        account: {
          id: '1',
          rewardBalance: 1500,
          status: AccountStatus.Active
        }
      }
    ];
    this.applyFilters();
  }

  onSearch(query: string): void {
    this.filter.searchQuery = query;
    this.applyFilters();
  }

  onStatusChange(status: string): void {
    this.filter.status = status === 'All' ? 'All' : status as AccountStatus;
    this.applyFilters();
  }

  onRoleChange(role: string): void {
    this.filter.role = role === 'All' ? 'All' : role as UserRole;
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = [...this.users];

    if (this.filter.status && this.filter.status !== 'All') {
      filtered = filtered.filter(user => user.account.status === this.filter.status);
    }

    if (this.filter.role && this.filter.role !== 'All') {
      filtered = filtered.filter(user => user.role === this.filter.role);
    }

    if (this.filter.searchQuery && this.filter.searchQuery.trim()) {
      const query = this.filter.searchQuery.toLowerCase();
      filtered = filtered.filter(user =>
        user.firstName.toLowerCase().includes(query) ||
        user.lastName.toLowerCase().includes(query) ||
        user.email.toLowerCase().includes(query) ||
        user.employeeId.toLowerCase().includes(query)
      );
    }

    if (this.filter.department) {
      filtered = filtered.filter(user => user.department === this.filter.department);
    }

    this.filteredUsers = filtered;
  }

  openCreateModal(): void {
    this.modalMode = 'create';
    this.userForm = this.getEmptyUserForm();
    this.selectedUser = null;
    this.showModal = true;
  }

  openEditModal(user: UserProfileDto): void {
    this.modalMode = 'edit';
    this.selectedUser = user;
    this.userForm = {
      employeeId: user.employeeId,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      role: user.role,
      department: user.department,
      phone: user.phone
    };
    this.showModal = true;
  }

  openPointsModal(user: UserProfileDto): void {
    this.selectedUser = user;
    this.pointsAdjustment = {
      points: 0,
      reason: '',
      type: 'add'
    };
    this.showPointsModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.userForm = this.getEmptyUserForm();
    this.selectedUser = null;
  }

  closePointsModal(): void {
    this.showPointsModal = false;
    this.selectedUser = null;
  }

  saveUser(): void {
    if (this.modalMode === 'create') {
      console.log('Creating user:', this.userForm);
      alert('User created successfully!');
    } else {
      console.log('Updating user:', this.userForm);
      alert('User updated successfully!');
    }
    this.closeModal();
  }

  adjustPoints(): void {
    if (!this.selectedUser) return;
    
    const adjustment = this.pointsAdjustment.type === 'add' 
      ? this.pointsAdjustment.points 
      : -this.pointsAdjustment.points;
    
    console.log('Adjusting points:', {
      user: this.selectedUser,
      adjustment,
      reason: this.pointsAdjustment.reason
    });
    
    alert(`Points ${this.pointsAdjustment.type === 'add' ? 'added' : 'deducted'} successfully!`);
    this.closePointsModal();
  }

  toggleUserStatus(user: UserProfileDto): void {
    const newStatus = user.account.status === AccountStatus.Active 
      ? AccountStatus.Inactive 
      : AccountStatus.Active;
    
    if (confirm(`Are you sure you want to ${newStatus === AccountStatus.Active ? 'activate' : 'deactivate'} ${user.firstName} ${user.lastName}?`)) {
      console.log('Toggling user status:', { user, newStatus });
      alert('User status updated successfully!');
    }
  }

  getUserStatusClass(status: AccountStatus): string {
    switch (status) {
      case AccountStatus.Active:
        return 'badge-active';
      case AccountStatus.Inactive:
        return 'badge-inactive';
      case AccountStatus.Suspended:
        return 'badge-suspended';
      default:
        return '';
    }
  }

  getUserRoleBadgeClass(role: UserRole): string {
    return role === UserRole.Admin ? 'role-admin' : 'role-user';
  }

  private getEmptyUserForm(): Partial<CreateUserRequest> {
    return {
      employeeId: '',
      firstName: '',
      lastName: '',
      email: '',
      role: UserRole.User,
      department: '',
      phone: ''
    };
  }

  clearFilters(): void {
    this.filter = {
      status: 'All',
      role: 'All',
      searchQuery: ''
    };
    this.selectedStatus = 'All';
    this.selectedRole = 'All';
    this.searchQuery = '';
    this.applyFilters();
  }
}
