import { EventStatus } from './enums';

export interface RewardRule {
  id: string;
  rank: number;
  rewardPointsId: string;
  pointsValue: number;
}

export interface Event {
  id: string; // Changed to string to match backend Guid
  eventId: string;
  name: string;
  description: string;
  firstPrize: number;
  secondPrize: number;
  thirdPrize: number;
  firstPrizeId?: string;
  secondPrizeId?: string;
  thirdPrizeId?: string;
  rewardRules?: RewardRule[];
  startDate: string;
  endDate: string;
  status: EventStatus;
  winnersAssigned?: boolean;
  location?: string;
  maxParticipants?: number;
  currentParticipants?: number;
  participantsCount?: number;
  participantCount?: number;  // For display purposes
  isParticipated?: boolean;  // Whether current user has participated
  createdAt?: string;
  createdBy?: string;
  category?: string;
}

/**
 * Event filter options
 */
export interface EventFilter {
  status?: EventStatus | 'All';
  startDate?: string;
  endDate?: string;
  searchQuery?: string;
  category?: string;
}

/**
 * Event participation model
 */
export interface EventParticipation {
  id: number;
  eventId: number;
  eventName: string;
  userId: number;
  userName: string;
  participationDate: string;
  rank?: number;
  pointsEarned: number;
  status: 'Registered' | 'Participated' | 'Winner';
}