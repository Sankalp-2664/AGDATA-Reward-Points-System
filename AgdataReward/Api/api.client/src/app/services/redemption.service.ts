import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { 
  Redemption, 
  RedemptionFilter, 
  CreateRedemptionRequest, 
  UpdateRedemptionStatus 
} from '../models/redemption.model';
import { RedemptionStatus } from '../models/enums';
import { environment } from '../../environments/environment';

export interface RedemptionRequestDto {
  userId: string;
  productId: string;
}

export interface RedemptionResponseDto {
  id: string;
  userId: string;
  productId: string;
  redeemedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class RedemptionService {
  private apiUrl = `${environment.apiUrl}/redemption`;
  
  private redemptionsSubject = new BehaviorSubject<Redemption[]>([
    {
      id: 1,
      redemptionId: 'RED-2025-001',
      userId: 1,
      userEmployeeId: 'AGD-2024-1523',
      userName: 'Sankalp Chakre',
      userEmail: 'sankalp.chakre@agdata.com',
      productId: 1,
      productName: 'Amazon Gift Card $50',
      productCode: 'PRD-001',
      pointsRequired: 500,
      quantity: 1,
      totalPoints: 500,
      status: RedemptionStatus.Delivered,
      requestedDate: '2025-01-10T14:20:00',
      approvedDate: '2025-01-10T15:00:00',
      deliveredDate: '2025-01-12T10:00:00',
      approvedBy: 'admin'
    },
    {
      id: 2,
      redemptionId: 'RED-2025-002',
      userId: 2,
      userEmployeeId: 'AGD-2024-1524',
      userName: 'Jane Smith',
      userEmail: 'jane.smith@agdata.com',
      productId: 2,
      productName: 'Wireless Mouse',
      productCode: 'PRD-002',
      pointsRequired: 300,
      quantity: 1,
      totalPoints: 300,
      status: RedemptionStatus.Pending,
      requestedDate: '2025-01-18T11:30:00'
    }
  ]);

  redemptions$: Observable<Redemption[]> = this.redemptionsSubject.asObservable();

  constructor(private http: HttpClient) {}

  getRedemptions(): Redemption[] {
    return this.redemptionsSubject.value;
  }

  getRedemptionsByUserId(userId: number): Redemption[] {
    return this.redemptionsSubject.value.filter(r => r.userId === userId);
  }

  getPendingRedemptions(): Redemption[] {
    return this.redemptionsSubject.value.filter(r => r.status === RedemptionStatus.Pending);
  }

  getFilteredRedemptions(filter: RedemptionFilter): Redemption[] {
    let redemptions = this.redemptionsSubject.value;

    if (filter.status && filter.status !== 'All') {
      redemptions = redemptions.filter(r => r.status === filter.status);
    }

    if (filter.userId) {
      redemptions = redemptions.filter(r => r.userId === filter.userId);
    }

    if (filter.startDate) {
      redemptions = redemptions.filter(r => r.requestedDate >= filter.startDate!);
    }

    if (filter.endDate) {
      redemptions = redemptions.filter(r => r.requestedDate <= filter.endDate!);
    }

    if (filter.searchQuery && filter.searchQuery.trim()) {
      const query = filter.searchQuery.toLowerCase();
      redemptions = redemptions.filter(r =>
        r.userName.toLowerCase().includes(query) ||
        r.productName.toLowerCase().includes(query) ||
        r.redemptionId.toLowerCase().includes(query) ||
        r.userEmail.toLowerCase().includes(query)
      );
    }

    return redemptions;
  }

  createRedemption(request: CreateRedemptionRequest, userId: number, userName: string, userEmail: string): void {
    const redemptions = this.redemptionsSubject.value;
    
    // Mock product lookup - replace with actual product service
    const mockProduct = {
      id: request.productId,
      name: 'Sample Product',
      code: 'PRD-XXX',
      points: 500
    };

    const newRedemption: Redemption = {
      id: redemptions.length + 1,
      redemptionId: `RED-${new Date().getFullYear()}-${(redemptions.length + 1).toString().padStart(3, '0')}`,
      userId,
      userEmployeeId: 'AGD-XXX',
      userName,
      userEmail,
      productId: request.productId,
      productName: mockProduct.name,
      productCode: mockProduct.code,
      pointsRequired: mockProduct.points,
      quantity: request.quantity,
      totalPoints: mockProduct.points * request.quantity,
      status: RedemptionStatus.Pending,
      requestedDate: new Date().toISOString()
    };

    this.redemptionsSubject.next([newRedemption, ...redemptions]);
  }

  updateRedemptionStatus(redemptionId: number, update: UpdateRedemptionStatus): void {
    const redemptions = this.redemptionsSubject.value;
    const index = redemptions.findIndex(r => r.id === redemptionId);

    if (index !== -1) {
      const updatedRedemption = { ...redemptions[index] };
      updatedRedemption.status = update.status;

      if (update.notes) {
        updatedRedemption.notes = update.notes;
      }

      if (update.rejectionReason) {
        updatedRedemption.rejectionReason = update.rejectionReason;
      }

      const now = new Date().toISOString();
      switch (update.status) {
        case RedemptionStatus.Approved:
          updatedRedemption.approvedDate = now;
          updatedRedemption.approvedBy = 'admin'; // Replace with actual admin user
          break;
        case RedemptionStatus.Rejected:
          updatedRedemption.rejectedDate = now;
          break;
        case RedemptionStatus.Delivered:
          updatedRedemption.deliveredDate = now;
          break;
      }

      const newRedemptions = [...redemptions];
      newRedemptions[index] = updatedRedemption;
      this.redemptionsSubject.next(newRedemptions);
    }
  }

  approveRedemption(redemptionId: number, notes?: string): void {
    this.updateRedemptionStatus(redemptionId, {
      status: RedemptionStatus.Approved,
      notes
    });
  }

  rejectRedemption(redemptionId: number, reason: string): void {
    this.updateRedemptionStatus(redemptionId, {
      status: RedemptionStatus.Rejected,
      rejectionReason: reason
    });
  }

  markAsDelivered(redemptionId: number): void {
    this.updateRedemptionStatus(redemptionId, {
      status: RedemptionStatus.Delivered
    });
  }

  cancelRedemption(redemptionId: number): void {
    this.updateRedemptionStatus(redemptionId, {
      status: RedemptionStatus.Cancelled
    });
  }

  // Statistics
  getRedemptionStats(): {
    total: number;
    pending: number;
    approved: number;
    rejected: number;
    delivered: number;
  } {
    const redemptions = this.redemptionsSubject.value;
    
    return {
      total: redemptions.length,
      pending: redemptions.filter(r => r.status === RedemptionStatus.Pending).length,
      approved: redemptions.filter(r => r.status === RedemptionStatus.Approved).length,
      rejected: redemptions.filter(r => r.status === RedemptionStatus.Rejected).length,
      delivered: redemptions.filter(r => r.status === RedemptionStatus.Delivered).length
    };
  }

  // Mock API call - replace with actual HTTP call
  async fetchRedemptions(): Promise<void> {
    console.log('Fetching redemptions from API...');
  }

  // ===== Backend API Methods =====

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ? `Bearer ${token}` : ''
    });
  }

  /**
   * Request a product redemption via backend API
   */
  requestRedemption(userId: string, productId: string): Observable<RedemptionResponseDto> {
    const request: RedemptionRequestDto = { userId, productId };
    return this.http.post<RedemptionResponseDto>(
      `${this.apiUrl}/request`,
      request,
      { headers: this.getAuthHeaders() }
    );
  }

  /**
   * Get user's redemption history from backend
   */
  getUserRedemptionHistory(userId: string): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/user/${userId}`,
      { headers: this.getAuthHeaders() }
    );
  }

  /**
   * Check if user has pending request for a product
   */
  hasPendingRequestForProduct(userId: string, productId: string): Observable<boolean> {
    return this.getUserRedemptionHistory(userId).pipe(
      map((history) => {
        const hasPending = history.some(
          (item: any) =>
            item.productId === productId &&
            item.status === 0 // Only Pending (0)
        );
        return hasPending;
      }),
      catchError(() => of(false))
    );
  }
}
