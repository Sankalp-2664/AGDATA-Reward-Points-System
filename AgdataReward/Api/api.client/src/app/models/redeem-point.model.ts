export interface RedeemPoint {
  id: string;  // Guid from backend
  pointsValue: number;  // renamed from 'value' to match backend
}

// For display purposes in the UI, keeping backward compatibility
export interface RedeemPointDisplay extends RedeemPoint {
  redeemPointId?: string;  // Optional display ID
}