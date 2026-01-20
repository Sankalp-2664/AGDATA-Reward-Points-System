import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService, User } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  currentUser: User | null = null;
  isEditing: boolean = false;
  formData = {
    firstName: '',
    lastName: '',
    email: ''
  };

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    console.log('📋 Profile: Loading user data...');
    // First, try to load user from backend
    this.userService.loadCurrentUser().subscribe({
      next: (user) => {
        console.log('✅ Profile: User data loaded successfully', user);
        this.currentUser = user;
        this.formData = {
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email
        };
      },
      error: (error) => {
        console.error('❌ Profile: Error loading user data from API:', error);
        console.error('Error status:', error.status);
        console.error('Error message:', error.message);
        
        // Check if it's a 404 or 401 error
        if (error.status === 404) {
          alert('User profile endpoint not found. Please ensure the backend server is running.');
        } else if (error.status === 401) {
          alert('Not authenticated. Please login again.');
          this.router.navigate(['/login']);
        }
        
        // Fallback to observable if direct load fails
        this.userService.currentUser$.subscribe(user => {
          if (user) {
            console.log('📦 Profile: Using cached user data', user);
            this.currentUser = user;
            this.formData = {
              firstName: user.firstName,
              lastName: user.lastName,
              email: user.email
            };
          }
        });
      }
    });
  }

  toggleEdit(): void {
    if (this.isEditing) {
      // Cancel editing - reset form
      if (this.currentUser) {
        this.formData = {
          firstName: this.currentUser.firstName,
          lastName: this.currentUser.lastName,
          email: this.currentUser.email
        };
      }
    }
    this.isEditing = !this.isEditing;
  }

  saveProfile(): void {
    if (!this.formData.firstName || !this.formData.lastName || !this.formData.email) {
      alert('Please fill all required fields');
      return;
    }

    if (!this.isValidEmail(this.formData.email)) {
      alert('Please enter a valid email address');
      return;
    }

    this.userService.updateUser(this.formData).subscribe({
      next: () => {
        this.isEditing = false;
        alert('Profile updated successfully!');
      },
      error: (error) => {
        console.error('Error updating profile:', error);
        alert('Failed to update profile. Please try again.');
      }
    });
  }

  isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  logout(): void {
    const confirm = window.confirm('Are you sure you want to logout?');
    if (confirm) {
      this.authService.logout();
      this.router.navigate(['/login']);
    }
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}