import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { WinnerAssignment, Participant, EventRank } from '../models/winner-assignment.model';

@Injectable({
  providedIn: 'root'
})
export class WinnerAssignmentService {
  private assignmentHistorySubject = new BehaviorSubject<WinnerAssignment[]>([]);
  assignmentHistory$ = this.assignmentHistorySubject.asObservable();

  private participantsSubject = new BehaviorSubject<Participant[]>([]);
  participants$ = this.participantsSubject.asObservable();

  constructor() {
    this.loadAssignmentHistory();
    this.loadParticipants();
  }

  private loadAssignmentHistory(): void {
    // TODO: Load from API - GET /api/events/{eventInstanceId}/winners
    this.assignmentHistorySubject.next([]);
  }

  private loadParticipants(): void {
    // TODO: Load from API - GET /api/events/{eventInstanceId}/participants
    this.participantsSubject.next([]);
  }

  getEventRanks(): EventRank[] {
    return [
      { rank: 1, prizePoints: 1000, medal: '🥇' },
      { rank: 2, prizePoints: 750, medal: '🥈' },
      { rank: 3, prizePoints: 500, medal: '🥉' }
    ];
  }

  assignWinner(
    eventInstanceId: string,
    userId: string,
    rank: number
  ): Promise<any> {
    // TODO: Call API - POST /api/events/assign-winner
    // Body: { eventInstanceId, userId, rank }
    return new Promise((resolve) => {
      const newAssignment: WinnerAssignment = {
        eventInstanceId,
        userId,
        rank
      };
      const current = this.assignmentHistorySubject.value;
      this.assignmentHistorySubject.next([...current, newAssignment]);
      resolve(newAssignment);
    });
  }

  getParticipants(eventInstanceId: string): Observable<Participant[]> {
    // TODO: Implement to fetch participants for specific event instance
    return this.participants$;
  }

  getAssignmentHistory(eventInstanceId: string): Observable<WinnerAssignment[]> {
    // TODO: Implement to fetch assignments for specific event instance
    return this.assignmentHistory$;
  }
}
