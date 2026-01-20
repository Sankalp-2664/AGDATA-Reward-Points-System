import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { UserService, User, Activity } from '../services/user.service';
import { EventService } from '../services/event.service';
import { AuthService } from '../services/auth.service';
import { Event } from '../models/event.model';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.css'],
})
export class UserDashboardComponent implements OnInit {
  currentUser: User | null = null;
  recentActivities: Activity[] = [];
  recentEvents: Event[] = [];
  pointsThisMonth: number = 0;
  rewardsRedeemed: number = 0;
  isAdmin: boolean = false;

  constructor(
    private userService: UserService,
    private eventService: EventService,
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadUserData();
    this.isAdmin = this.authService.isAdmin();
  }

  loadUserData(): void {
    // Load current user from backend
    this.userService.loadCurrentUser().subscribe({
      next: (user) => {
        this.currentUser = user;
        this.pointsThisMonth = this.userService.getPointsThisMonth();
        this.rewardsRedeemed = this.userService.getRewardsRedeemed();
      },
      error: (error) => {
        console.error('Error loading user:', error);
      }
    });

    // Subscribe to user updates
    this.userService.currentUser$.subscribe((user) => {
      if (user) {
        this.currentUser = user;
      }
    });

    this.userService.activities$.subscribe((activities) => {
      this.recentActivities = activities.slice(0, 3);
      this.pointsThisMonth = this.userService.getPointsThisMonth();
      this.rewardsRedeemed = this.userService.getRewardsRedeemed();
    });

    this.eventService.events$.subscribe((events) => {
      this.recentEvents = events
        .filter((e) => e.status === 'Active')
        .slice(0, 2);
    });
  }

  navigateToHome(): void {
    this.router.navigate(['/user/home']);
  }

  navigateToEvents(): void {
    this.router.navigate(['/user/events']);
  }

  navigateToProducts(): void {
    this.router.navigate(['/user/products']);
  }

  navigateToRewards(): void {
    this.router.navigate(['/user/my-rewards']);
  }

  navigateToAdmin(): void {
    if (this.authService.isAdmin()) {
      this.router.navigate(['/admin']);
    } else {
      alert('Access denied: Admin privileges required');
    }
  }

  navigateToProfile(): void {
    this.router.navigate(['/user/profile']);
  }
}
