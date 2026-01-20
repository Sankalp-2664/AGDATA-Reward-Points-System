import { Routes } from '@angular/router';
import { LoginformComponent } from './loginform/loginform.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { UserDashboardComponent } from './user-dashboard/user-dashboard.component';
import { HomeComponent } from './user-dashboard/home/home.component';
import { EventsComponent } from './user-dashboard/events/events.component';
import { ProductsComponent } from './user-dashboard/products/products.component';
import { MyRewardsComponent } from './user-dashboard/my-rewards/my-rewards.component';
import { ProfileComponent } from './user-dashboard/profile/profile.component';
import { adminGuard } from './guards/admin.guard';  

export const APP_ROUTES: Routes = [
  // Public
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginformComponent },

  // User area
  {
    path: 'user',
    component: UserDashboardComponent,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      { path: 'dashboard', component: HomeComponent },
      { path: 'events', component: EventsComponent },
      { path: 'products', component: ProductsComponent },
      { path: 'my-rewards', component: MyRewardsComponent },
      { path: 'profile', component: ProfileComponent }
    ]
  },

  // Admin area
  {
    path: 'admin',
    component: AdminDashboardComponent,
    canActivate: [adminGuard]
  },

  // Fallback
  { path: '**', redirectTo: 'login' }
];
