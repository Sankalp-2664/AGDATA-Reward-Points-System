import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { UserProfileDto } from '../models/user-profile.model';

export interface User {
  id: string;
  employeeId: string;
  firstName: string;
  lastName: string;
  email: string;
  points: number;
  status: 'Active' | 'Inactive';
  roles?: string[];
}

export interface Activity {
  id: number;
  description: string;
  points: number;
  date: string;
  type: 'earned' | 'redeemed' | 'pending' | 'rejected';
  status?: string;
  productName?: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);

  private activitiesSubject = new BehaviorSubject<Activity[]>([]);

  currentUser$: Observable<User> = this.currentUserSubject.asObservable() as Observable<User>;
  activities$: Observable<Activity[]> = this.activitiesSubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Load current user from the backend
   */
  loadCurrentUser(): Observable<User> {
    console.log('🔄 Loading current user from:', `${this.apiUrl}/users/me`);
    return this.http.get<UserProfileDto>(`${this.apiUrl}/users/me`).pipe(
      tap(userDto => console.log('✅ Received user data:', userDto)),
      map(userDto => this.mapUserProfileToUser(userDto)),
      tap(user => {
        console.log('✅ Mapped user:', user);
        this.currentUserSubject.next(user);
        // Also load user's activities when user is loaded
        this.loadUserActivities(user.id);
      })
    );
  }

  /**
   * Load user activities/transactions and redemption history
   */
  private loadUserActivities(userId: string): void {
    // Load transactions from rewards API
    this.http.get<any[]>(`${this.apiUrl}/reward/user/${userId}`).pipe(
      map(transactions => this.mapTransactionsToActivities(transactions))
    ).subscribe({
      next: (activities) => {
        // Also load redemption history to get pending items
        this.loadRedemptionHistory(userId, activities);
      },
      error: (error) => {
        console.error('Error loading activities:', error);
        // Try to load redemption history even if transactions fail
        this.loadRedemptionHistory(userId, []);
      }
    });
  }

  /**
   * Load user redemption history and merge with activities
   */
  private loadRedemptionHistory(userId: string, existingActivities: Activity[]): void {
    this.http.get<any[]>(`${this.apiUrl}/redemption/user/${userId}`).subscribe({
      next: (redemptions) => {
        const pendingActivities = this.mapRedemptionsToActivities(redemptions);
        // Merge and deduplicate (transactions may already have redemptions as 'redeemed')
        const allActivities = [...existingActivities, ...pendingActivities];
        this.activitiesSubject.next(allActivities);
      },
      error: (error) => {
        console.error('Error loading redemption history:', error);
        this.activitiesSubject.next(existingActivities);
      }
    });
  }

  /**
   * Map redemption history to Activity interface
   */
  private mapRedemptionsToActivities(redemptions: any[]): Activity[] {
    return redemptions.map((r, index) => {
      // status: 0 = Pending, 1 = Approved, 2 = Rejected, 3 = Completed
      let type: 'pending' | 'redeemed' | 'rejected' = 'pending';
      let statusText = 'Pending';
      let description = r.productName;

      if (r.status === 0) {
        type = 'pending';
        statusText = 'Pending';
        description = `Pending: ${r.productName}`;
      } else if (r.status === 1 || r.status === 3) {
        // Approved or Completed
        type = 'redeemed';
        statusText = 'Redeemed';
        description = `Redeemed: ${r.productName}`;
      } else if (r.status === 2) {
        // Rejected
        type = 'rejected';
        statusText = 'Rejected';
        description = `Rejected: ${r.productName}`;
      }

      return {
        id: 10000 + index, // Use high IDs to avoid collision
        description,
        points: -r.pointsUsed,
        date: r.createdAt,
        type,
        status: statusText,
        productName: r.productName
      };
    });
  }

  /**
   * Map UserProfileDto to User interface
   */
  private mapUserProfileToUser(userDto: UserProfileDto): User {
    // Handle roles - backend returns it as 'roles' array
    let roles: string[] = [];
    if (userDto.roles && Array.isArray(userDto.roles)) {
      roles = userDto.roles;
    } else if (userDto.role) {
      roles = Array.isArray(userDto.role) ? userDto.role : [userDto.role as string];
    }

    return {
      id: userDto.id,
      employeeId: userDto.employeeId,
      firstName: userDto.firstName,
      lastName: userDto.lastName,
      email: userDto.email,
      points: userDto.account?.rewardBalance || 0,
      status: userDto.account?.status === 'Active' ? 'Active' : 'Inactive',
      roles: roles
    };
  }

  /**
   * Map backend transactions to Activity interface
   */
  private mapTransactionsToActivities(transactions: any[]): Activity[] {
    return transactions.map((tx, index) => {
      // Determine transaction type: 1 = Credit (earned), 2 = Debit (redeemed)
      const isCredit = tx.transactionType === 1 || tx.transactionType === 'Credit';
      let points = tx.pointsDelta || tx.amount || tx.points || 0;
      
      // Ensure positive points for earned (credit) transactions
      if (isCredit) {
        points = Math.abs(points);
      } else {
        // Ensure negative points for redeemed (debit) transactions
        points = -Math.abs(points);
      }
      
      // Use notes directly - backend already includes event names
      let description = tx.notes || this.getTransactionDescription(tx);
      
      return {
        id: index + 1,
        description,
        points,
        date: tx.createdAt || tx.transactionDate || tx.date || new Date().toISOString(),
        type: isCredit ? 'earned' : 'redeemed'
      };
    });
  }

  /**
   * Get a friendly description for a transaction
   */
  private getTransactionDescription(tx: any): string {
    if (tx.transactionType === 'Earned' || tx.type === 'earned') {
      return tx.eventName || 'Points Earned';
    } else {
      return tx.productName || 'Points Redeemed';
    }
  }

  getCurrentUser(): User {
    return this.currentUserSubject.value!;
  }

  updateUser(userData: Partial<User>): Observable<any> {
    const currentUser = this.currentUserSubject.value;
    if (!currentUser) {
      throw new Error('No user logged in');
    }

    const updatePayload = {
      firstName: userData.firstName || currentUser.firstName,
      lastName: userData.lastName || currentUser.lastName,
      email: userData.email || currentUser.email,
      role: currentUser.roles?.[0] || 'User',
      accountStatus: currentUser.status
    };

    return this.http.put<UserProfileDto>(`${this.apiUrl}/users/${currentUser.id}`, updatePayload).pipe(
      tap(updatedUserDto => {
        const updatedUser = this.mapUserProfileToUser(updatedUserDto);
        this.currentUserSubject.next(updatedUser);
      })
    );
  }

  getActivities(): Activity[] {
    return this.activitiesSubject.value;
  }

  getRewardsRedeemed(): number {
    return this.activitiesSubject.value.filter(a => a.type === 'redeemed').length;
  }

  getPointsThisMonth(): number {
    const thisMonth = new Date().getMonth();
    const thisYear = new Date().getFullYear();
    
    return this.activitiesSubject.value
      .filter(a => {
        const activityDate = new Date(a.date);
        return a.type === 'earned' && 
               activityDate.getMonth() === thisMonth && 
               activityDate.getFullYear() === thisYear;
      })
      .reduce((sum, activity) => sum + activity.points, 0);
  }
}