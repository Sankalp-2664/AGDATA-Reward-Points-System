import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Admin Guard - Protects routes that should only be accessible by admins
 * Redirects non-admin users to the home page
 */
export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Check if user is logged in
  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  // Check if user has admin role
  if (!authService.isAdmin()) {
    console.warn('Access denied: Admin privileges required');
    router.navigate(['/user/home']);
    return false;
  }

  return true;
};
