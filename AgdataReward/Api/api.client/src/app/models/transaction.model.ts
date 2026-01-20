import { TransactionType } from './enums';

/**
 * Transaction model representing points transactions
 */
export interface Transaction {
  id: number;
  transactionId: string;
  userId: number;
  userEmployeeId: string;
  userName: string;
  type: TransactionType;
  points: number;
  description: string;
  eventId?: number;
  eventName?: string;
  productId?: number;
  productName?: string;
  redemptionId?: number;
  createdAt: string;
  createdBy?: string;
}

/**
 * Transaction filter options
 */
export interface TransactionFilter {
  type?: TransactionType | 'All';
  startDate?: string;
  endDate?: string;
  searchQuery?: string;
  userId?: number;
}

/**
 * Transaction summary for dashboard statistics
 */
export interface TransactionSummary {
  totalEarned: number;
  totalRedeemed: number;
  currentBalance: number;
  transactionCount: number;
  thisMonthEarned: number;
  thisMonthRedeemed: number;
}
