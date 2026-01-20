import { AccountStatus, UserRole } from './enums';

export interface UserAccountDto {
  id: string;
  rewardBalance: number;
  status: AccountStatus | string; // Backend returns string
  totalEarned?: number;
  totalRedeemed?: number;
  lastTransactionDate?: string;
}

export interface UserProfileDto {
  id: string;
  employeeId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole | string | string[]; // Support both single role and array
  roles?: string[]; // Backend returns roles array
  department?: string;
  joinDate?: string;
  phone?: string;
  account?: UserAccountDto; // Make optional to match backend
}

/**
 * User filter options
 */
export interface UserFilter {
  status?: AccountStatus | 'All';
  role?: UserRole | 'All';
  searchQuery?: string;
  department?: string;
}

/**
 * Create user request DTO
 */
export interface CreateUserRequest {
  employeeId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  department?: string;
  phone?: string;
}

/**
 * Update user request DTO
 */
export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  email?: string;
  department?: string;
  phone?: string;
  status?: AccountStatus;
  role?: UserRole;
}
