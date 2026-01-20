import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService, RewardPoints } from '../../services/event.service';
import { UsersService } from '../../services/users.service';
import { UserProfileDto } from '../../models/user-profile.model';
import { Event, EventFilter } from '../../models/event.model';
import { EventStatus } from '../../models/enums';
import { SearchFilterComponent } from '../../shared/components/search-filter.component';
import { StatusFilterComponent } from '../../shared/components/status-filter.component';
import { DateRangeFilterComponent, DateRangeFilter } from '../../shared/components/date-range-filter.component';

@Component({
  selector: 'app-admin-events',
  standalone: true,
  imports: [CommonModule, FormsModule, SearchFilterComponent, StatusFilterComponent, DateRangeFilterComponent],
  templateUrl: './admin-events.component.html',
  styleUrls: ['./admin-events.component.css']
})
export class AdminEventsComponent implements OnInit {
  events: Event[] = [];
  filteredEvents: Event[] = [];
  rewardPointsList: RewardPoints[] = [];
  usersList: UserProfileDto[] = [];
  
  filter: EventFilter = {
    status: 'All',
    searchQuery: '',
    startDate: '',
    endDate: ''
  };
  
  statusOptions: string[] = ['All', EventStatus.Active, EventStatus.Upcoming, EventStatus.Completed, EventStatus.Cancelled];
  selectedStatus: string = 'All';
  searchQuery: string = '';
  
  showModal: boolean = false;
  modalMode: 'create' | 'edit' = 'create';
  selectedEvent: Event | null = null;
  
  // Winner assignment modal
  showWinnersModal: boolean = false;
  selectedEventForWinners: Event | null = null;
  winnersForm = {
    firstPlaceUserId: '',
    secondPlaceUserId: '',
    thirdPlaceUserId: ''
  };
  
  eventForm: Partial<Event> = this.getEmptyEventForm();

  constructor(
    private eventService: EventService,
    private usersService: UsersService
  ) {}

  ngOnInit(): void {
    // Subscribe to events observable first
    this.eventService.events$.subscribe(events => {
      console.log('📥 Component received events from service:', events);
      console.log('🔢 Number of events:', events.length);
      if (events.length > 0) {
        console.log('🔍 First event:', events[0]);
      }
      this.events = events;
      this.applyFilters();
    });
    
    // Then load events from API
    this.loadEvents();
    this.loadRewardPoints();
    this.loadUsers();
  }

  loadEvents(): void {
    console.log('🔄 loadEvents called - fetching from API...');
    this.eventService.getAllEvents().subscribe({
      next: () => {
        console.log('✅ Events loaded successfully');
      },
      error: (err) => {
        console.error('❌ Error loading events:', err);
      }
    });
  }

  loadRewardPoints(): void {
    this.eventService.rewardPoints$.subscribe(points => {
      this.rewardPointsList = points;
      console.log('Loaded reward points in component:', points);
    });
  }

  loadUsers(): void {
    this.usersService.getAllUsers().subscribe({
      next: (users) => {
        this.usersList = users;
        console.log('👥 Loaded users:', users);
      },
      error: (err) => {
        console.error('❌ Error loading users:', err);
      }
    });
  }

  onSearch(query: string): void {
    this.filter.searchQuery = query;
    this.applyFilters();
  }

  onStatusChange(status: string): void {
    this.filter.status = status === 'All' ? 'All' : status as EventStatus;
    this.applyFilters();
  }

  onDateRangeChange(dateRange: DateRangeFilter): void {
    this.filter.startDate = dateRange.startDate;
    this.filter.endDate = dateRange.endDate;
    this.applyFilters();
  }

  applyFilters(): void {
    console.log('🔍 applyFilters called');
    console.log('  - this.events.length:', this.events.length);
    console.log('  - this.filter:', this.filter);
    
    let filtered = [...this.events];
    console.log('  - Initial filtered count:', filtered.length);

    if (this.filter.status && this.filter.status !== 'All') {
      filtered = filtered.filter(event => event.status === this.filter.status);
      console.log(`  - After status filter (${this.filter.status}):`, filtered.length);
    }

    if (this.filter.searchQuery && this.filter.searchQuery.trim()) {
      const query = this.filter.searchQuery.toLowerCase();
      filtered = filtered.filter(event =>
        event.name.toLowerCase().includes(query) ||
        event.eventId.toLowerCase().includes(query) ||
        event.description.toLowerCase().includes(query)
      );
      console.log(`  - After search filter (${query}):`, filtered.length);
    }

    if (this.filter.startDate) {
      filtered = filtered.filter(event => event.startDate >= this.filter.startDate!);
      console.log(`  - After startDate filter:`, filtered.length);
    }
    if (this.filter.endDate) {
      filtered = filtered.filter(event => event.endDate <= this.filter.endDate!);
      console.log(`  - After endDate filter:`, filtered.length);
    }

    this.filteredEvents = filtered;
    console.log('✅ Final filteredEvents.length:', this.filteredEvents.length);
    if (this.filteredEvents.length > 0) {
      console.log('  - First filtered event:', this.filteredEvents[0]);
    }
  }

  openCreateModal(): void {
    this.modalMode = 'create';
    this.eventForm = this.getEmptyEventForm();
    this.selectedEvent = null;
    this.showModal = true;
  }

  openEditModal(event: Event): void {
    this.modalMode = 'edit';
    this.selectedEvent = event;
    this.eventForm = { 
      ...event,
      firstPrizeId: event.firstPrizeId,
      secondPrizeId: event.secondPrizeId,
      thirdPrizeId: event.thirdPrizeId
    };
    this.showModal = true;
  }

  saveEvent(): void {
    console.log('💾 saveEvent called');
    console.log('📋 eventForm:', this.eventForm);
    console.log('EventID:', this.eventForm.eventId);
    console.log('Name:', this.eventForm.name);
    console.log('StartDate:', this.eventForm.startDate);
    console.log('EndDate:', this.eventForm.endDate);
    console.log('🎁 Prize IDs:', {
      firstPrizeId: this.eventForm.firstPrizeId,
      secondPrizeId: this.eventForm.secondPrizeId,
      thirdPrizeId: this.eventForm.thirdPrizeId
    });
    
    // Validate required fields
    if (!this.eventForm.eventId || !this.eventForm.name || !this.eventForm.startDate || !this.eventForm.endDate) {
      const msg = `Please fill in all required fields (Code, Name, Start Date, End Date). Current values - Code: "${this.eventForm.eventId || ''}", Name: "${this.eventForm.name || ''}", StartDate: "${this.eventForm.startDate || ''}", EndDate: "${this.eventForm.endDate || ''}"`;  
      console.error('❌ Validation failed:', msg);
      alert(msg);
      return;
    }

    const eventData: Event = {
      id: this.eventForm.id || '',
      eventId: this.eventForm.eventId || '',
      name: this.eventForm.name || '',
      description: this.eventForm.description || this.eventForm.name || '',
      firstPrize: this.eventForm.firstPrize || 0,
      secondPrize: this.eventForm.secondPrize || 0,
      thirdPrize: this.eventForm.thirdPrize || 0,
      firstPrizeId: this.eventForm.firstPrizeId,
      secondPrizeId: this.eventForm.secondPrizeId,
      thirdPrizeId: this.eventForm.thirdPrizeId,
      startDate: this.eventForm.startDate || '',
      endDate: this.eventForm.endDate || '',
      status: this.eventForm.status || EventStatus.Upcoming,
      location: this.eventForm.location,
      maxParticipants: this.eventForm.maxParticipants,
      category: this.eventForm.category
    };
    
    console.log('📤 Sending eventData:', eventData);

    if (this.modalMode === 'create') {
      this.eventService.addEvent(eventData).subscribe({
        next: () => {
          console.log('✅ Event created successfully, reloading events...');
          alert('Event created successfully!');
          this.closeModal();
          // Reload events to show the new event
          this.loadEvents();
        },
        error: (err) => {
          console.error('❌ Error creating event:', err);
          alert(err?.error?.message || 'Failed to create event');
        }
      });
    } else {
      this.eventService.updateEvent(eventData).subscribe({
        next: () => {
          console.log('✅ Event updated successfully, reloading events...');
          alert('Event updated successfully!');
          this.closeModal();
          // Reload events to show the updated event
          this.loadEvents();
        },
        error: (err) => {
          console.error('❌ Error updating event:', err);
          alert(err?.error?.message || 'Failed to update event');
        }
      });
    }
  }

  closeModal(): void {
    this.showModal = false;
  }

  deleteEvent(event: Event): void {
    if (confirm(`Are you sure you want to delete event "${event.name}"?`)) {
      console.log('Deleting event:', event);
      alert('Event deleted successfully!');
    }
  }

  getEventStatusClass(status: EventStatus): string {
    switch (status) {
      case EventStatus.Active:
        return 'badge-active';
      case EventStatus.Upcoming:
        return 'badge-upcoming';
      case EventStatus.Completed:
        return 'badge-completed';
      case EventStatus.Cancelled:
        return 'badge-cancelled';
      default:
        return '';
    }
  }

  private getEmptyEventForm(): Partial<Event> {
    return {
      eventId: '',
      name: '',
      description: '',
      firstPrize: 0,
      secondPrize: 0,
      thirdPrize: 0,
      firstPrizeId: undefined,
      secondPrizeId: undefined,
      thirdPrizeId: undefined,
      startDate: '',
      endDate: '',
      status: EventStatus.Upcoming,
      location: '',
      maxParticipants: 0,
      category: ''
    };
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

  // Winner assignment modal methods
  openAssignWinnersModal(event: Event): void {
    this.selectedEventForWinners = event;
    this.winnersForm = {
      firstPlaceUserId: '',
      secondPlaceUserId: '',
      thirdPlaceUserId: ''
    };
    this.showWinnersModal = true;
  }

  closeWinnersModal(): void {
    this.showWinnersModal = false;
    this.selectedEventForWinners = null;
  }

  assignWinnersAndComplete(): void {
    if (!this.selectedEventForWinners) return;

    const eventId = this.selectedEventForWinners.id;
    const { firstPlaceUserId, secondPlaceUserId, thirdPlaceUserId } = this.winnersForm;

    console.log('🏆 Assigning winners:', {
      eventId,
      firstPlaceUserId: firstPlaceUserId || null,
      secondPlaceUserId: secondPlaceUserId || null,
      thirdPlaceUserId: thirdPlaceUserId || null
    });

    this.eventService.completeEventWithWinners(
      eventId,
      firstPlaceUserId || null,
      secondPlaceUserId || null,
      thirdPlaceUserId || null
    ).subscribe({
      next: () => {
        alert('Event completed and winners assigned successfully! Points have been awarded.');
        this.closeWinnersModal();
      },
      error: (err) => {
        console.error('❌ Error completing event:', err);
        alert(err?.error?.message || 'Failed to complete event');
      }
    });
  }

  // Mark event as completed without winners
  markEventCompleted(event: Event): void {
    if (confirm(`Are you sure you want to mark "${event.name}" as completed?`)) {
      this.eventService.updateEventStatus(event.id, 'Completed').subscribe({
        next: () => {
          alert('Event marked as completed!');
        },
        error: (err) => {
          console.error('❌ Error updating event status:', err);
          alert(err?.error?.message || 'Failed to update event status');
        }
      });
    }
  }
}
