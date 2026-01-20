import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, map, tap, of } from 'rxjs';
import { RedeemPoint } from '../models/redeem-point.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RedeemPointService {
  private apiUrl = `${environment.apiUrl}/reward/points`;
  private redeemPointsSubject = new BehaviorSubject<RedeemPoint[]>([]);
  redeemPoints$: Observable<RedeemPoint[]> = this.redeemPointsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadRedeemPoints();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ? `Bearer ${token}` : ''
    });
  }

  /**
   * Load all reward points from backend
   */
  loadRedeemPoints(): void {
    this.http.get<RedeemPoint[]>(this.apiUrl, { headers: this.getAuthHeaders() })
      .pipe(
        tap(points => console.log('Loaded redeem points:', points)),
        catchError(error => {
          console.error('Error loading redeem points:', error);
          return of([]);
        })
      )
      .subscribe(points => {
        this.redeemPointsSubject.next(points);
      });
  }

  getRedeemPoints(): RedeemPoint[] {
    return this.redeemPointsSubject.value;
  }

  /**
   * Update reward point value
   */
  updateRedeemPointValue(id: string, newValue: number): void {
    const updateDto = { pointsValue: newValue };
    
    this.http.put<RedeemPoint>(`${this.apiUrl}/${id}`, updateDto, { headers: this.getAuthHeaders() })
      .pipe(
        tap(updated => console.log('Updated redeem point:', updated)),
        catchError(error => {
          console.error('Error updating redeem point:', error);
          alert('Failed to update redeem point. Please try again.');
          throw error;
        })
      )
      .subscribe(updated => {
        // Update local state
        const redeemPoints = this.redeemPointsSubject.value.map(rp =>
          rp.id === id ? updated : rp
        );
        this.redeemPointsSubject.next(redeemPoints);
      });
  }

  /**
   * Add a new reward point configuration
   */
  addRedeemPoint(pointsValue: number): void {
    const createDto = { pointsValue };
    
    this.http.post<RedeemPoint>(this.apiUrl, createDto, { headers: this.getAuthHeaders() })
      .pipe(
        tap(created => console.log('Created redeem point:', created)),
        catchError(error => {
          console.error('Error creating redeem point:', error);
          alert('Failed to create redeem point. Please try again.');
          throw error;
        })
      )
      .subscribe(created => {
        // Add to local state
        const redeemPoints = [...this.redeemPointsSubject.value, created];
        this.redeemPointsSubject.next(redeemPoints);
      });
  }

  /**
   * Get reward point by ID
   */
  getRedeemPointById(id: string): Observable<RedeemPoint | null> {
    return this.http.get<RedeemPoint>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() })
      .pipe(
        catchError(error => {
          console.error('Error fetching redeem point:', error);
          return of(null);
        })
      );
  }

  /**
   * Get reward point value by ID
   */
  getRedeemPointValueById(id: string): Observable<number | undefined> {
    return this.getRedeemPointById(id).pipe(
      map(rp => rp?.pointsValue)
    );
  }

  // For users/admin to fetch value by ID - optimized for quick lookup
  fetchRedeemPointValueById(id: string): Observable<number | undefined> {
    return this.getRedeemPointValueById(id);
  }

  // Alternative: Get full redeem point details by ID
  fetchRedeemPointById(id: string): Observable<RedeemPoint | null> {
    return this.getRedeemPointById(id);
  }
}