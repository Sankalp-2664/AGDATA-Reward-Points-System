import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, forkJoin, of } from 'rxjs';
import { tap, switchMap, map, catchError } from 'rxjs/operators';
import { type Event, RewardRule } from '../models/event.model';
import { EventStatus } from '../models/enums';
import { environment } from '../../environments/environment';

export interface RewardPoints {
  id: string;
  pointsValue: number;
}

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = `${environment.apiUrl}/event`;
  private eventsSubject = new BehaviorSubject<Event[]>([]);
  events$: Observable<Event[]> = this.eventsSubject.asObservable();
  
  private rewardPointsSubject = new BehaviorSubject<RewardPoints[]>([]);
  rewardPoints$: Observable<RewardPoints[]> = this.rewardPointsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadRewardPoints();
  }

  loadRewardPoints(): void {
    this.http.get<RewardPoints[]>('http://localhost:5010/api/reward/points').pipe(
      tap(points => {
        console.log('📊 Loaded reward points:', points);
        this.rewardPointsSubject.next(points);
      })
    ).subscribe();
  }

  private mapStatusFromApi(apiStatus: string): EventStatus {
    switch (apiStatus?.toLowerCase()) {
      case 'active': return EventStatus.Active;
      case 'upcoming': return EventStatus.Upcoming;
      case 'completed': return EventStatus.Completed;
      case 'cancelled': return EventStatus.Cancelled;
      default: return EventStatus.Upcoming;
    }
  }

  // GET /api/event/with-rewards - Get all events with reward details
  getAllEvents(): Observable<Event[]> {
    console.log('🎉 Calling GET /api/event/with-rewards endpoint...');
    return this.http.get<any[]>(`${this.apiUrl}/with-rewards`).pipe(
      tap(events => {
        console.log('📥 Received events from API:', events);
        console.log('🔍 Number of events:', events.length);
        
        const transformedEvents = events.map((evt: any, index: number) => {
          console.log(`🔄 Transforming event ${index}:`, evt);
          console.log(`  - id: ${evt.id}`);
          console.log(`  - code: ${evt.code}`);
          console.log(`  - title: ${evt.title}`);
          console.log(`  - status: ${evt.status}`);
          console.log(`  - winnersAssigned: ${evt.winnersAssigned}`);
          console.log(`  - startDate: ${evt.startDate}`);
          console.log(`  - endDate: ${evt.endDate}`);
          console.log(`  - participantsCount: ${evt.participantsCount}`);
          console.log(`  - rewardRules:`, evt.rewardRules);
          
          // Extract reward rules
          const rewardRules: RewardRule[] = evt.rewardRules || [];
          const rule1 = rewardRules.find((r: RewardRule) => r.rank === 1);
          const rule2 = rewardRules.find((r: RewardRule) => r.rank === 2);
          const rule3 = rewardRules.find((r: RewardRule) => r.rank === 3);
          
          const transformed: Event = {
            id: evt.id,
            eventId: evt.code,
            name: evt.title,
            description: evt.title,
            firstPrize: rule1?.pointsValue || 0,
            secondPrize: rule2?.pointsValue || 0,
            thirdPrize: rule3?.pointsValue || 0,
            firstPrizeId: rule1?.rewardPointsId,
            secondPrizeId: rule2?.rewardPointsId,
            thirdPrizeId: rule3?.rewardPointsId,
            rewardRules: rewardRules,
            startDate: evt.startDate ? new Date(evt.startDate).toISOString().split('T')[0] : '',
            endDate: evt.endDate ? new Date(evt.endDate).toISOString().split('T')[0] : '',
            status: this.mapStatusFromApi(evt.status),
            winnersAssigned: evt.winnersAssigned || false,
            participantsCount: evt.participantsCount || 0
          };
          console.log(`✅ Transformed event ${index}:`, transformed);
          return transformed;
        });
        console.log('✅ Setting events to subject:', transformedEvents);
        this.eventsSubject.next(transformedEvents);
      })
    );
  }

  getEvents(): Event[] {
    return this.eventsSubject.value;
  }

  // POST /api/event - Add new event (includes reward rules)
  addEvent(event: Event): Observable<any> {
    console.log('🎉 Creating event:', event);
    const payload = {
      code: event.eventId,
      title: event.name,
      startDate: event.startDate,
      endDate: event.endDate,
      // Send direct prize values (backend will create RewardPoints entries)
      firstPrize: event.firstPrize || 0,
      secondPrize: event.secondPrize || 0,
      thirdPrize: event.thirdPrize || 0,
      // Also include reward points IDs if available (for backward compatibility)
      firstPrizeRewardPointsId: event.firstPrizeId || null,
      secondPrizeRewardPointsId: event.secondPrizeId || null,
      thirdPrizeRewardPointsId: event.thirdPrizeId || null
    };
    console.log('📤 Sending payload to backend:', payload);
    return this.http.post(`${this.apiUrl}`, payload).pipe(
      tap((response: any) => {
        console.log('✅ Event created successfully with reward rules:', response);
      }),
      switchMap(() => this.getAllEvents()),
      tap(() => console.log('✅ Events reloaded'))
    );
  }

  // POST /api/event/{eventId}/reward-rule - Add reward rule
  private addRewardRule(eventId: string, rank: number, rewardPointsId: string): Observable<any> {
    const payload = {
      rank: rank,
      rewardPointsId: rewardPointsId
    };
    console.log(`📬 POST /api/event/${eventId}/reward-rule`, payload);
    return this.http.post(`${this.apiUrl}/${eventId}/reward-rule`, payload).pipe(
      tap(response => console.log(`✅ Reward rule created for rank ${rank}:`, response)),
      tap({error: (err: any) => console.error(`❌ Error creating reward rule for rank ${rank}:`, err)})
    );
  }

  // PUT /api/event/reward-rule/{ruleId} - Update reward rule
  private updateRewardRule(ruleId: string, rewardPointsId: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/reward-rule/${ruleId}`, {
      rewardPointsId: rewardPointsId
    });
  }

  // PUT /api/event/{id} - Update event (includes reward rules)
  updateEvent(event: Event): Observable<any> {
    console.log('🔄 Updating event:', event);
    const payload = {
      id: event.id,
      code: event.eventId,
      title: event.name,
      startDate: event.startDate,
      endDate: event.endDate,
      status: event.status,
      // Send direct prize values (backend will create/update RewardPoints entries)
      firstPrize: event.firstPrize || 0,
      secondPrize: event.secondPrize || 0,
      thirdPrize: event.thirdPrize || 0,
      // Also include reward points IDs if available (for backward compatibility)
      firstPrizeRewardPointsId: event.firstPrizeId || null,
      secondPrizeRewardPointsId: event.secondPrizeId || null,
      thirdPrizeRewardPointsId: event.thirdPrizeId || null
    };
    console.log('📤 Sending update payload to backend:', payload);
    return this.http.put(`${this.apiUrl}/${event.id}`, payload).pipe(
      tap((response: any) => {
        console.log('✅ Event updated successfully with reward rules:', response);
      }),
      switchMap(() => this.getAllEvents()),
      tap(() => console.log('✅ Events reloaded'))
    );
  }

  // Assign winners to an event
  assignWinners(eventId: string, firstPrizeWinnerId: number, secondPrizeWinnerId: number, thirdPrizeWinnerId: number): void {
    const events = this.eventsSubject.value.map(evt =>
      evt.id === eventId ? { ...evt, status: EventStatus.Completed } : evt
    );
    this.eventsSubject.next(events);
    // In a real implementation, this would send data to the backend
    console.log('Winners assigned:', { eventId, firstPrizeWinnerId, secondPrizeWinnerId, thirdPrizeWinnerId });
  }

  // PUT /api/event/{id}/status - Update event status
  updateEventStatus(eventId: string, status: string): Observable<any> {
    console.log(`🔄 Updating event ${eventId} status to ${status}`);
    return this.http.put(`${this.apiUrl}/${eventId}/status`, { status }).pipe(
      tap(() => {
        console.log('✅ Event status updated successfully');
      }),
      switchMap(() => this.getAllEvents())
    );
  }

  // POST /api/event/{id}/complete - Complete event with winners
  completeEventWithWinners(
    eventId: string, 
    firstPlaceUserId: string | null, 
    secondPlaceUserId: string | null, 
    thirdPlaceUserId: string | null
  ): Observable<any> {
    console.log(`🏆 Completing event ${eventId} with winners:`, {
      firstPlaceUserId,
      secondPlaceUserId,
      thirdPlaceUserId
    });
    return this.http.post(`${this.apiUrl}/${eventId}/complete`, {
      firstPlaceUserId: firstPlaceUserId || null,
      secondPlaceUserId: secondPlaceUserId || null,
      thirdPlaceUserId: thirdPlaceUserId || null
    }).pipe(
      tap(() => {
        console.log('✅ Event completed and winners assigned successfully');
      }),
      switchMap(() => this.getAllEvents())
    );
  }

  // Get event by ID
  getEventById(eventId: string): Event | undefined {
    return this.eventsSubject.value.find(e => e.id === eventId);
  }

  // Get participant count for an event
  getParticipantCount(eventId: string): Observable<number> {
    // Return 0 immediately - we'll get count from the event data itself
    return of(0);
  }

  // Participate in event
  participateInEvent(eventInstanceId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${eventInstanceId}/participate`, {}).pipe(
      tap(() => {
        console.log('✅ Successfully participated in event');
        // Refresh events to get updated participant count
        this.getAllEvents().subscribe();
      }),
      catchError((error: any) => {
        console.error('❌ Error participating in event:', error);
        throw error;
      })
    );
  }
}