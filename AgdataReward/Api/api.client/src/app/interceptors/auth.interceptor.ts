import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();

    console.log('🔐 Auth Interceptor - Token exists:', !!token);
    if (token) {
      console.log('🔐 Token preview:', token.substring(0, 50) + '...');
    }
    console.log('🌐 Request URL:', request.url);
    console.log('📍 Request method:', request.method);

    // Clone the request and add authorization header if token exists
    if (token) {
      console.log('✅ Adding Authorization header to request');
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      console.log('✅ Authorization header added');
    } else {
      console.warn('⚠️ No token found - request will be sent without Authorization header');
      console.log('📦 LocalStorage auth_token:', localStorage.getItem('auth_token'));
      console.log('📦 LocalStorage token:', localStorage.getItem('token'));
    }

    return next.handle(request);
  }
}
