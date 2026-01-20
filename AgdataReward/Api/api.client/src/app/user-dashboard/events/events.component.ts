import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { UserService } from '../../services/user.service';
import { Event, EventFilter } from '../../models/event.model';
import { EventStatus } from '../../models/enums';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.css']
})
export class EventsComponent implements OnInit {
  events: Event[] = [];
  filteredEvents: Event[] = [];
  searchQuery: string = '';
  filterDropdownOpen: boolean = false;
  participatedEventIds: Set<string> = new Set();
  
  // Filter properties
  filter: EventFilter = {
    status: 'All',
    searchQuery: '',
    startDate: '',
    endDate: ''
  };
  
  statusOptions: string[] = ['All', 'Active', 'Upcoming', 'Completed'];
  selectedStatus: string = 'All';

  constructor(
    private eventService: EventService,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.eventService.events$.subscribe(events => {
      this.events = events.map(event => ({
        ...event,
        participantCount: event.participantsCount || event.currentParticipants || 0
      }));
      this.applyFilters();
    });
  }

  onSearch(query: string): void {
    this.filter.searchQuery = query;
    this.applyFilters();
  }

  toggleFilterDropdown(): void {
    this.filterDropdownOpen = !this.filterDropdownOpen;
  }

  selectStatus(status: string): void {
    this.selectedStatus = status;
    this.filter.status = status === 'All' ? 'All' : status as EventStatus;
    this.filterDropdownOpen = false;
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = [...this.events];

    // Status filter
    if (this.filter.status && this.filter.status !== 'All') {
      filtered = filtered.filter(event => event.status === this.filter.status);
    }

    // Search filter
    if (this.filter.searchQuery && this.filter.searchQuery.trim()) {
      const query = this.filter.searchQuery.toLowerCase();
      filtered = filtered.filter(event =>
        event.name.toLowerCase().includes(query) ||
        event.eventId.toLowerCase().includes(query) ||
        event.description.toLowerCase().includes(query)
      );
    }

    this.filteredEvents = filtered;
  }

  participateInEvent(event: Event): void {
    if (event.status !== 'Active' || this.isParticipated(event.id)) {
      return;
    }

    // Call backend API to participate
    this.eventService.participateInEvent(event.id).subscribe({
      next: () => {
        this.participatedEventIds.add(event.id);
        alert(`Successfully participating in: ${event.name}`);
        // Reload events to get updated participant count
        this.loadEvents();
      },
      error: (err) => {
        console.error('Error participating in event:', err);
        alert(err?.error?.message || 'Failed to participate in event. Please try again.');
      }
    });
  }

  isParticipated(eventId: string): boolean {
    // Check from backend data first
    const event = this.events.find(e => e.id === eventId);
    if (event && event.isParticipated) {
      return true;
    }
    // Fallback to local state for optimistic UI update
    return this.participatedEventIds.has(eventId);
  }

  clearFilters(): void {
    this.filter = {
      status: 'All',
      searchQuery: '',
      startDate: '',
      endDate: ''
    };
    this.selectedStatus = 'All';
    this.searchQuery = '';
    this.applyFilters();
  }

  getEventStatusBadgeClass(status: EventStatus): string {
    switch (status) {
      case EventStatus.Active:
        return 'status-active';
      case EventStatus.Upcoming:
        return 'status-upcoming';
      case EventStatus.Completed:
        return 'status-completed';
      default:
        return '';
    }
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}