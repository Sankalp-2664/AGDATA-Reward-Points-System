import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { environment } from '../../environments/environment';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  userId: string;
  role?: string;
}

export interface AuthState {
  token: string | null;
  expiresAt: string | null;
  isLoggedIn: boolean;
  role: string | null;
  userId: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private authStateSubject = new BehaviorSubject<AuthState>({
    token: this.getStoredToken(),
    expiresAt: this.getStoredExpiresAt(),
    isLoggedIn: !!this.getStoredToken(),
    role: this.getStoredRole(),
    userId: this.getStoredUserId()
  });

  public authState$ = this.authStateSubject.asObservable();

  constructor(private http: HttpClient) {
    // Initialize auth state from localStorage
    this.initializeAuthState();
  }

  /**
   * Initialize auth state from localStorage on service creation
   */
  private initializeAuthState(): void {
    const token = this.getStoredToken();
    const expiresAt = this.getStoredExpiresAt();
    
    if (token && expiresAt) {
      const isTokenValid = new Date(expiresAt) > new Date();
      if (isTokenValid) {
        const role = this.extractRoleFromToken(token);
        const userId = this.getStoredUserId();
        if (role) {
          localStorage.setItem('user_role', role);
        }
        this.authStateSubject.next({
          token,
          expiresAt,
          isLoggedIn: true,
          role,
          userId
        });
      } else {
        // Token expired, clear it
        this.logout();
      }
    }
  }

  /**
   * Login user with email and password
   */
  login(email: string, password: string): Observable<AuthResponse> {

  const payload = {
    email: email.trim().toLowerCase(),
    password: password.trim()
  };

  console.log("LOGIN REQUEST:", payload);

  return this.http.post<AuthResponse>(`${this.apiUrl}/login`, payload).pipe(
    tap((response) => {
      localStorage.setItem('auth_token', response.token);
      localStorage.setItem('token', response.token); // Also store as 'token' for redemption service
      localStorage.setItem('auth_expires_at', response.expiresAt);
      localStorage.setItem('userId', response.userId);

      // Extract role from JWT token
      const role = this.extractRoleFromToken(response.token);
      if (role) {
        localStorage.setItem('user_role', role);
      }

      this.authStateSubject.next({
        token: response.token,
        expiresAt: response.expiresAt,
        isLoggedIn: true,
        role,
        userId: response.userId
      });
    }),
    catchError((error) => {
      console.log("LOGIN ERROR FULL:", error);
      return throwError(() => new Error(error?.error?.message || "Invalid credentials"));
    })
  );
}


  /**
   * Logout user by clearing token and auth state
   */
  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('token');
    localStorage.removeItem('auth_expires_at');
    localStorage.removeItem('userId');
    localStorage.removeItem('user_role');
    
    this.authStateSubject.next({
      token: null,
      expiresAt: null,
      isLoggedIn: false,
      role: null,
      userId: null
    });
  }

  /**
   * Get current auth token
   */
  getToken(): string | null {
    return this.getStoredToken();
  }

  /**
   * Check if user is logged in
   */
  isLoggedIn(): boolean {
    const token = this.getStoredToken();
    const expiresAt = this.getStoredExpiresAt();
    
    if (!token || !expiresAt) {
      return false;
    }

    const isTokenValid = new Date(expiresAt) > new Date();
    if (!isTokenValid) {
      this.logout();
      return false;
    }

    return true;
  }

  /**
   * Get current auth state
   */
  getAuthState(): AuthState {
    return this.authStateSubject.value;
  }

  /**
   * Check if current user is admin
   */
  isAdmin(): boolean {
    const role = this.getStoredRole();
    return role === 'Admin';
  }

  /**
   * Get current user role
   */
  getUserRole(): string | null {
    return this.getStoredRole();
  }

  /**
   * Get current user ID
   */
  getUserId(): string | null {
    return this.getStoredUserId();
  }

  /**
   * Extract role from JWT token
   */
  private extractRoleFromToken(token: string): string | null {
    try {
      const payload = token.split('.')[1];
      const decoded = JSON.parse(atob(payload));
      // JWT uses 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' for roles
      return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded['role'] || null;
    } catch (e) {
      console.error('Error decoding token:', e);
      return null;
    }
  }

  /**
   * Retrieve stored token from localStorage
   */
  private getStoredToken(): string | null {
    if (typeof localStorage === 'undefined') {
      return null;
    }
    return localStorage.getItem('auth_token');
  }

  /**
   * Retrieve stored expiration time from localStorage
   */
  private getStoredExpiresAt(): string | null {
    if (typeof localStorage === 'undefined') {
      return null;
    }
    return localStorage.getItem('auth_expires_at');
  }

  /**
   * Retrieve stored role from localStorage
   */
  private getStoredRole(): string | null {
    if (typeof localStorage === 'undefined') {
      return null;
    }
    return localStorage.getItem('user_role');
  }

  /**
   * Retrieve stored user ID from localStorage
   */
  private getStoredUserId(): string | null {
    if (typeof localStorage === 'undefined') {
      return null;
    }
    return localStorage.getItem('userId');
  }
}
