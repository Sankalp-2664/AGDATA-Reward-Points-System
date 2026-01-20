import { RedemptionStatus } from './enums';

/**
 * Redemption request model
 */
export interface Redemption {
  id: number;
  redemptionId: string;
  userId: number;
  userEmployeeId: string;
  userName: string;
  userEmail: string;
  productId: number;
  productName: string;
  productCode: string;
  pointsRequired: number;
  quantity: number;
  totalPoints: number;
  status: RedemptionStatus;
  requestedDate: string;
  approvedDate?: string;
  rejectedDate?: string;
  deliveredDate?: string;
  rejectionReason?: string;
  approvedBy?: string;
  notes?: string;
}

/**
 * Redemption filter options
 */
export interface RedemptionFilter {
  status?: RedemptionStatus | 'All';
  startDate?: string;
  endDate?: string;
  searchQuery?: string;
  userId?: number;
}

/**
 * Create redemption request DTO
 */
export interface CreateRedemptionRequest {
  productId: number;
  quantity: number;
}

/**
 * Update redemption status DTO
 */
export interface UpdateRedemptionStatus {
  status: RedemptionStatus;
  notes?: string;
  rejectionReason?: string;
}
