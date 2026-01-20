import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { UserProfileDto, UserAccountDto } from '../models/user-profile.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private apiUrl = `${environment.apiUrl}/users`;
  private usersSubject = new BehaviorSubject<UserProfileDto[]>([]);
  public users$ = this.usersSubject.asObservable();

  constructor(private http: HttpClient) {}

  // GET /api/users/me - Get current logged in user
  getCurrentUser(): Observable<UserProfileDto> {
    console.log('🌐 Calling GET /api/users/me endpoint...');
    return this.http.get<UserProfileDto>(`${this.apiUrl}/me`).pipe(
      tap(user => console.log('📥 Received current user from API:', user))
    );
  }

  // GET /api/users - Get all users
  getAllUsers(): Observable<UserProfileDto[]> {
    console.log('🌐 Calling GET /api/users endpoint...');
    console.log('API URL:', `${this.apiUrl}`);
    return this.http.get<UserProfileDto[]>(`${this.apiUrl}`).pipe(
      tap(users => {
        console.log('📥 Received users from API:', users);
        this.usersSubject.next(users);
      })
    );
  }

  //  GET /api/users/{id}
  getUserById(id: string): Observable<UserProfileDto> {
    return this.http.get<UserProfileDto>(`${this.apiUrl}/${id}`);
  }

  //  POST /api/users
  createUser(payload: {
    employeeId: string;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
    password: string;
  }): Observable<UserProfileDto> {
    return this.http.post<UserProfileDto>(`${this.apiUrl}`, payload);
  }

  // ✅ GET /api/users/{id}/account
  getUserAccount(id: string): Observable<UserAccountDto> {
    return this.http.get<UserAccountDto>(`${this.apiUrl}/${id}/account`);
  }

  // PUT /api/users/{id} - Update user
  updateUser(id: string, payload: {
    firstName: string;
    lastName: string;
    email: string;
    role: string;
    accountStatus: string;
  }): Observable<UserProfileDto> {
    console.log('🌐 Calling PUT /api/users/' + id, payload);
    return this.http.put<UserProfileDto>(`${this.apiUrl}/${id}`, payload).pipe(
      tap(user => console.log('📥 Updated user response:', user))
    );
  }
}
