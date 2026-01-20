export interface WinnerAssignment {
  eventInstanceId: string;
  userId: string;
  rank: number;
}

export interface EventRank {
  rank: number;
  prizePoints: number;
  medal: string;
}

export interface Participant {
  id: string;
  name: string;
  email: string;
  currentPoints: number;
}
