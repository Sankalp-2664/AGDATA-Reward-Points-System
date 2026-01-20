/**
 * Enums for AGDATA Reward Points System
 * Centralized enum definitions for type safety
 */

export enum AccountStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Suspended = 'Suspended'
}

export enum RedemptionStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled'
}

export enum TransactionType {
  Earned = 'Earned',
  Redeemed = 'Redeemed',
  Adjusted = 'Adjusted',
  Refunded = 'Refunded'
}

export enum EventStatus {
  Active = 'Active',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  Upcoming = 'Upcoming'
}

export enum ProductStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  OutOfStock = 'OutOfStock'
}

export enum UserRole {
  Admin = 'Admin',
  User = 'User'
}
