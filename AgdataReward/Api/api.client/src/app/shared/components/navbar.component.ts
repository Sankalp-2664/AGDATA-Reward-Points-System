// import { Component, OnInit } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { RouterModule, Router } from '@angular/router';
// import { AuthService, User } from '../../core/services/auth.service';

// /**
//  * Global Navigation Bar Component
//  * Displays on all pages with navigation links and user menu
//  */
// @Component({
//   selector: 'app-navbar',
//   standalone: true,
//   imports: [CommonModule, RouterModule],
//   template: `
//     <nav class="ag-navbar">
//       <div class="ag-navbar-container">
//         <!-- Logo -->
//         <div class="ag-navbar-brand">
//           <a routerLink="/dashboard" class="ag-navbar-logo">
//             <span class="logo-icon">🏆</span>
//             <span class="logo-text">AGDATA Reward</span>
//           </a>
//         </div>

//         <!-- Menu Items -->
//         <div class="ag-navbar-menu">
//           <a routerLink="/dashboard" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }" class="ag-navbar-item">
//             Dashboard
//           </a>
//           <a routerLink="/events" routerLinkActive="active" class="ag-navbar-item">
//             Events
//           </a>
//           <a routerLink="/products" routerLinkActive="active" class="ag-navbar-item">
//             Products
//           </a>
//           <a routerLink="/rewards" routerLinkActive="active" class="ag-navbar-item">
//             My Rewards
//           </a>
//           <a 
//             (click)="navigateToAdmin()" 
//             [class.disabled]="!isAdmin"
//             class="ag-navbar-item"
//             [title]="isAdmin ? '' : 'Only admin can access this section'"
//           >
//             Admin
//           </a>
//         </div>

//         <!-- Right Section: User Menu -->
//         <div class="ag-navbar-right">
//           <div class="ag-user-info" *ngIf="currentUser">
//             <span class="ag-user-greeting">{{ currentUser.firstName }}</span>
//             <button class="ag-user-avatar" (click)="toggleUserMenu()" [attr.aria-label]="'User menu'">
//               {{ getInitials(currentUser) }}
//             </button>
//           </div>

//           <!-- User Dropdown Menu -->
//           <div class="ag-user-menu" *ngIf="showUserMenu">
//             <a routerLink="/profile" (click)="closeUserMenu()" class="ag-user-menu-item">
//               👤 Profile
//             </a>
//             <hr class="ag-menu-divider">
//             <button (click)="logout()" class="ag-user-menu-item logout">
//               🚪 Logout
//             </button>
//           </div>
//         </div>
//       </div>
//     </nav>
//   `,
//   styles: [`
//     .ag-navbar {
//       background-color: var(--ag-color-white);
//       border-bottom: 1px solid var(--ag-color-gray-100);
//       box-shadow: var(--ag-shadow-sm);
//       position: sticky;
//       top: 0;
//       z-index: var(--ag-z-sticky);
//     }

//     .ag-navbar-container {
//       max-width: 1400px;
//       margin: 0 auto;
//       padding: 0 var(--ag-spacing-lg);
//       display: flex;
//       align-items: center;
//       height: 64px;
//       gap: var(--ag-spacing-xl);
//     }

//     .ag-navbar-brand {
//       flex-shrink: 0;
//     }

//     .ag-navbar-logo {
//       display: flex;
//       align-items: center;
//       gap: var(--ag-spacing-sm);
//       text-decoration: none;
//       color: var(--ag-color-primary-dark);
//       transition: color var(--ag-transition-base);
//     }

//     .ag-navbar-logo:hover {
//       color: var(--ag-color-primary);
//     }

//     .logo-icon {
//       font-size: 24px;
//     }

//     .logo-text {
//       font: var(--ag-heading-05);
//       font-weight: 700;
//     }

//     .ag-navbar-menu {
//       display: flex;
//       gap: var(--ag-spacing-lg);
//       flex: 1;
//       align-items: center;
//     }

//     .ag-navbar-item {
//       color: var(--ag-color-gray-600);
//       text-decoration: none;
//       font: var(--ag-body-01);
//       padding: var(--ag-spacing-sm) var(--ag-spacing-md);
//       border-radius: var(--ag-radius-md);
//       transition: all var(--ag-transition-base);
//       cursor: pointer;
//       border: none;
//       background: none;
//     }

//     .ag-navbar-item:hover:not(.disabled) {
//       color: var(--ag-color-primary);
//       background-color: var(--ag-color-primary-lightest);
//     }

//     .ag-navbar-item.active {
//       color: var(--ag-color-primary);
//       font-weight: 600;
//       background-color: var(--ag-color-primary-lightest);
//     }

//     .ag-navbar-item.disabled {
//       opacity: 0.6;
//       cursor: not-allowed;
//     }

//     .ag-navbar-right {
//       display: flex;
//       align-items: center;
//       gap: var(--ag-spacing-md);
//       position: relative;
//       flex-shrink: 0;
//     }

//     .ag-user-info {
//       display: flex;
//       align-items: center;
//       gap: var(--ag-spacing-sm);
//     }

//     .ag-user-greeting {
//       color: var(--ag-color-gray-600);
//       font: var(--ag-body-01);
//       display: none;
//     }

//     @media (min-width: 768px) {
//       .ag-user-greeting {
//         display: inline;
//       }
//     }

//     .ag-user-avatar {
//       width: 40px;
//       height: 40px;
//       border-radius: 50%;
//       background-color: var(--ag-color-primary);
//       color: white;
//       border: none;
//       cursor: pointer;
//       font: var(--ag-body-02);
//       font-size: 14px;
//       transition: all var(--ag-transition-base);
//       display: flex;
//       align-items: center;
//       justify-content: center;
//     }

//     .ag-user-avatar:hover {
//       background-color: var(--ag-color-primary-dark);
//       transform: scale(1.05);
//     }

//     .ag-user-menu {
//       position: absolute;
//       top: 100%;
//       right: 0;
//       margin-top: var(--ag-spacing-sm);
//       background-color: var(--ag-color-white);
//       border-radius: var(--ag-radius-lg);
//       box-shadow: var(--ag-shadow-lg);
//       min-width: 200px;
//       z-index: var(--ag-z-dropdown);
//       animation: slideDown 200ms ease-out;
//     }

//     @keyframes slideDown {
//       from {
//         opacity: 0;
//         transform: translateY(-10px);
//       }
//       to {
//         opacity: 1;
//         transform: translateY(0);
//       }
//     }

//     .ag-user-menu-item {
//       display: block;
//       width: 100%;
//       padding: var(--ag-spacing-md);
//       text-align: left;
//       color: var(--ag-color-gray-600);
//       text-decoration: none;
//       border: none;
//       background: none;
//       cursor: pointer;
//       font: var(--ag-body-01);
//       transition: all var(--ag-transition-base);
//     }

//     .ag-user-menu-item:first-child {
//       border-radius: var(--ag-radius-lg) var(--ag-radius-lg) 0 0;
//     }

//     .ag-user-menu-item:hover {
//       background-color: var(--ag-color-gray-50);
//       color: var(--ag-color-primary);
//     }

//     .ag-user-menu-item.logout {
//       border-radius: 0 0 var(--ag-radius-lg) var(--ag-radius-lg);
//       color: var(--ag-color-error);
//     }

//     .ag-user-menu-item.logout:hover {
//       background-color: var(--ag-color-error-lightest);
//     }

//     .ag-menu-divider {
//       margin: 0;
//       border: none;
//       border-top: 1px solid var(--ag-color-gray-100);
//     }

//     @media (max-width: 768px) {
//       .ag-navbar-container {
//         gap: var(--ag-spacing-md);
//         padding: 0 var(--ag-spacing-md);
//       }

//       .ag-navbar-menu {
//         gap: var(--ag-spacing-md);
//       }

//       .ag-navbar-item {
//         padding: var(--ag-spacing-xs) var(--ag-spacing-sm);
//         font-size: 14px;
//       }
//     }
//   `]
// })
// export class NavbarComponent implements OnInit {
//   currentUser: User | null = null;
//   showUserMenu = false;
//   isAdmin = false;

//   constructor(
//     private authService: AuthService,
//     private router: Router
//   ) {}

//   ngOnInit(): void {
//     this.currentUser = this.authService.getCurrentUser();
//     this.isAdmin = this.authService.isAdmin();
//   }

//   getInitials(user: User): string {
//     return (user.firstName.charAt(0) + user.lastName.charAt(0)).toUpperCase();
//   }

//   toggleUserMenu(): void {
//     this.showUserMenu = !this.showUserMenu;
//   }

//   closeUserMenu(): void {
//     this.showUserMenu = false;
//   }

//   navigateToAdmin(): void {
//     if (this.isAdmin) {
//       this.router.navigate(['/admin']);
//     } else {
//       alert('Only admin can access this section');
//     }
//   }

//   logout(): void {
//     this.authService.logout();
//     this.router.navigate(['/login']);
//   }
// }
