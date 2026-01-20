import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { PendingRequest } from '../models/pending-request.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RequestService {
  private apiUrl = `${environment.apiUrl}/redemption`;
  private requestsSubject = new BehaviorSubject<PendingRequest[]>([]);
  requests$: Observable<PendingRequest[]> = this.requestsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadPendingRequests();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ? `Bearer ${token}` : ''
    });
  }

  loadPendingRequests(): void {
    this.http.get<any[]>(`${this.apiUrl}/pending`, { headers: this.getAuthHeaders() })
      .subscribe({
        next: (response) => {
          const mappedRequests: PendingRequest[] = response.map((item: any) => ({
            id: item.id,  // Use RedemptionRequest.Id (not redemptionId which is RedemptionRecord.Id)
            employeeId: item.employeeId,
            name: item.userName,
            email: item.userEmail,
            product: item.productName,
            points: item.pointsUsed,
            requestedDate: new Date(item.createdAt).toISOString().split('T')[0]
          }));
          this.requestsSubject.next(mappedRequests);
        },
        error: (err) => {
          console.error('Error loading pending requests:', err);
          this.requestsSubject.next([]);
        }
      });
  }

  getRequests(): PendingRequest[] {
    return this.requestsSubject.value;
  }

  approveRequest(requestId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${requestId}/approve`, {}, { headers: this.getAuthHeaders() })
      .pipe(
        tap(() => {
          // Reload pending requests after approval
          this.loadPendingRequests();
        })
      );
  }

  rejectRequest(requestId: string, reason: string): Observable<any> {
    console.log(`Rejecting request ${requestId}. Reason: ${reason}`);
    return this.http.post(`${this.apiUrl}/${requestId}/reject`, {}, { headers: this.getAuthHeaders() })
      .pipe(
        tap(() => {
          // Reload pending requests after rejection
          this.loadPendingRequests();
        })
      );
  }
}