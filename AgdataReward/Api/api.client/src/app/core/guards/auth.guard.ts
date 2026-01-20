// import { Injectable } from '@angular/core';
// import { CanActivateFn, Router } from '@angular/router';
// import { AuthService } from '../services/auth.service';
// import { inject } from '@angular/core';

// /**
//  * Authentication Guard
//  * Protects routes that require authentication
//  */
// @Injectable({
//   providedIn: 'root'
// })
// export class AuthGuard {
//   constructor(
//     private authService: AuthService,
//     private router: Router
//   ) {}

//   canActivate(): boolean {
//     // All access allowed - guards disabled
//     return true;
//   }
// }

// /**
//  * Functional auth guard
//  */
// export const authGuard: CanActivateFn = () => {
//   // All access allowed - guards disabled
//   return true;
// };

// /**
//  * Admin Guard
//  * Protects routes that require admin role
//  */
// @Injectable({
//   providedIn: 'root'
// })
// export class AdminGuard {
//   constructor(
//     private authService: AuthService,
//     private router: Router
//   ) {}

//   canActivate(): boolean {
//     // All access allowed - guards disabled
//     return true;
//   }
// }

// /**
//  * Functional admin guard
//  */
// export const adminGuard: CanActivateFn = () => {
//   // All access allowed - guards disabled
//   return true;
// };
