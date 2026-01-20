import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Event } from '../../models/event.model';
import { UserProfileDto, UserAccountDto } from '../../models/user-profile.model';
import { EventService } from '../../services/event.service';
import { UsersService } from '../../services/users.service';

export interface WinnerAssignment {
  eventId: number;
  firstPrizeWinnerId: number;
  secondPrizeWinnerId: number;
  thirdPrizeWinnerId: number;
  firstPrizeWinnerName?: string;
  secondPrizeWinnerName?: string;
  thirdPrizeWinnerName?: string;
}

@Component({
  selector: 'app-assign-winner',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './assign-winner.component.html',
  styleUrls: ['./assign-winner.component.css']
})
export class AssignWinnerComponent implements OnInit {
  event: Event | null = null;
  users: UserProfileDto[] = [];

  winners: WinnerAssignment = {
    eventId: 0,
    firstPrizeWinnerId: 0,
    secondPrizeWinnerId: 0,
    thirdPrizeWinnerId: 0
  };

  loading = false;
  submitted = false;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private usersService: UsersService
  ) {}

  ngOnInit(): void {
    this.loadEventData();
    this.loadUsers();
  }

  loadEventData(): void {
    this.route.queryParams.subscribe(params => {
      const eventId = params['eventId'];
      if (eventId) {
        this.eventService.events$.subscribe(events => {
          this.event = events.find(e => e.id === parseInt(eventId, 10)) || null;
          if (this.event) {
            this.winners.eventId = this.event.id;
          }
        });
      }
    });
  }

  loadUsers(): void {
    this.usersService.users$.subscribe(users => {
      this.users = users.filter(user => user.status === 'Active');
    });
  }

  getWinnerName(winnerId: number): string {
    return this.users.find(user => user.id === winnerId)?.name || '';
  }

  onWinnerChange(position: 'first' | 'second' | 'third'): void {
    switch (position) {
      case 'first':
        this.winners.firstPrizeWinnerName =
          this.getWinnerName(this.winners.firstPrizeWinnerId);
        break;
      case 'second':
        this.winners.secondPrizeWinnerName =
          this.getWinnerName(this.winners.secondPrizeWinnerId);
        break;
      case 'third':
        this.winners.thirdPrizeWinnerName =
          this.getWinnerName(this.winners.thirdPrizeWinnerId);
        break;
    }
  }

  validateForm(): boolean {
    if (this.winners.firstPrizeWinnerId === 0) {
      this.errorMessage = 'First prize winner is required';
      return false;
    }
    if (this.winners.secondPrizeWinnerId === 0) {
      this.errorMessage = 'Second prize winner is required';
      return false;
    }
    if (this.winners.thirdPrizeWinnerId === 0) {
      this.errorMessage = 'Third prize winner is required';
      return false;
    }

    const winnerIds = [
      this.winners.firstPrizeWinnerId,
      this.winners.secondPrizeWinnerId,
      this.winners.thirdPrizeWinnerId
    ];

    const uniqueIds = new Set(winnerIds);
    if (uniqueIds.size !== winnerIds.length) {
      this.errorMessage = 'Same user cannot be assigned multiple prizes';
      return false;
    }

    this.errorMessage = '';
    return true;
  }

  submitWinners(): void {
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;
    this.submitted = true;

    // Call service to save winners
    // this.eventService.assignWinners(this.winners).subscribe({
    //   next: () => {
    //     alert('Winners assigned successfully!');
    //     this.router.navigate(['/admin-dashboard']);
    //   },
    //   error: (err) => {
    //     this.errorMessage = 'Error assigning winners: ' + err.message;
    //     this.loading = false;
    //   }
    // });

    console.log('Winners to assign:', this.winners);
    setTimeout(() => {
      alert('Winners assigned successfully!');
      this.router.navigate(['/admin-dashboard']);
    }, 1000);
  }

  cancelAssignment(): void {
    this.router.navigate(['/admin-dashboard']);
  }
}
