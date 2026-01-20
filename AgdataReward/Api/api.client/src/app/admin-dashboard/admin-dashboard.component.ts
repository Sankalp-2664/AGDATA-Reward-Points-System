import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Router } from '@angular/router';

import { UserAccountDto, UserProfileDto } from '../models/user-profile.model';
import { Event } from '../models/event.model';
import { Product } from '../models/product.model';
import { RedeemPoint } from '../models/redeem-point.model';
import { PendingRequest } from '../models/pending-request.model';
import { EventStatus, ProductStatus } from '../models/enums';

import { UsersService } from '../services/users.service';
import { EventService } from '../services/event.service';
import { ProductService } from '../services/product.service';
import { RedeemPointService } from '../services/redeem-point.service';
import { RequestService } from '../services/request.service';
import { WinnerAssignmentService } from '../services/winner-assignment.service';
import { EventRank, Participant } from '../models/winner-assignment.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css'],
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  stats = {
    users: 0,
    points: 0,
    redeemed: 0,
    pendingRequests: 0,
  };

  tabs: string[] = [
    'Approve Request',
    'User Management',
    'Event Management',
    'Product Management',
    'Redeem Points Management',
  ];
  activeTab: number = 0;

  // ✅ UPDATED: users instead of employees
  users: UserProfileDto[] = [];
  selectedUserAccount: UserAccountDto | null = null;

  pendingRequests: PendingRequest[] = [];
  events: Event[] = [];
  products: Product[] = [];
  redeemPoints: RedeemPoint[] = [];
  participants: Participant[] = [];
  eventRanks: EventRank[] = [];
  rewardPointsList: any[] = [];

  showModal: boolean = false;
  modalType: string = '';
  selectedItem: any = null;
  rejectReason: string = '';
  formData: any = {};

  private destroy$ = new Subject<void>();

  constructor(
    private router: Router,
    private usersService: UsersService,
    private eventService: EventService,
    private productService: ProductService,
    private redeemPointService: RedeemPointService,
    private requestService: RequestService,
    private winnerAssignmentService: WinnerAssignmentService,
  ) {
    this.eventRanks = this.winnerAssignmentService.getEventRanks();
  }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadData(): void {
    // Requests
    this.requestService.requests$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (requests: PendingRequest[]) => {
        this.pendingRequests = requests;
        this.stats.pendingRequests = requests.length;
      },
      error: (err: any) => console.error('Error loading requests:', err),
    });

    // Events - subscribe to the observable and trigger initial load
    this.eventService.events$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (events: Event[]) => {
          console.log('✅ Events loaded in component:', events);
          console.log('✅ Events count:', events.length);
          if (events.length > 0) {
            console.log('✅ First event:', events[0]);
          }
          this.events = events;
          console.log('✅ this.events after assignment:', this.events);
        },
        error: (err: any) => console.error('❌ Error loading events:', err),
      });
    
    // Trigger initial load of events from API
    this.eventService.getAllEvents().subscribe();

    // Products
    this.productService.products$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (products: Product[]) => {
        this.products = products;
      },
      error: (err: any) => console.error('Error loading products:', err),
    });
    
    // Reward Points
    this.productService.rewardPoints$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (rewardPoints: any[]) => {
        this.rewardPointsList = rewardPoints;
      },
      error: (err: any) => console.error('Error loading reward points:', err),
    });

    // Redeem Points
    this.redeemPointService.redeemPoints$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (redeemPoints: RedeemPoint[]) => {
          this.redeemPoints = redeemPoints;
        },
        error: (err: any) => console.error('Error loading redeem points:', err),
      });

    // ✅ Users list
    console.log('🔍 Attempting to load users...');
    this.usersService.getAllUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users: UserProfileDto[]) => {
          console.log('✅ Users loaded successfully:', users);
          console.log('📊 Number of users:', users.length);
          this.users = users;
          this.stats.users = users.length;
        },
        error: (err: any) => {
          console.error('❌ Error loading users:', err);
          console.error('Error details:', err.error);
          console.error('Status:', err.status);
        },
      });
  }

  setTab(index: number): void {
    if (index >= 0 && index < this.tabs.length) {
      this.activeTab = index;
    }
  }

  // Modal Handlers
  openModal(type: string, item: any = null): void {
    this.modalType = type;
    this.selectedItem = item;
    
    // Handle user editing specially to populate form correctly
    if (type === 'editUser' && item) {
      this.formData = {
        employeeId: item.employeeId,
        firstName: item.firstName,
        lastName: item.lastName,
        email: item.email,
        role: item.roles && item.roles.length > 0 ? item.roles[0] : 'User',
        accountStatus: item.account?.status || 'Active',
      };
    } else if (type === 'updateProduct' && item) {
      // Initialize update product form with current values
      const matchingRewardPoint = this.rewardPointsList.find(rp => rp.pointsValue === item.points);
      this.formData = {
        name: item.name,
        stock: item.stock,
        status: item.status,
        rewardPointsId: matchingRewardPoint?.id || (this.rewardPointsList.length > 0 ? this.rewardPointsList[0].id : '')
      };
    } else if (type === 'updateEvent' && item) {
      // Initialize event update form with current values
      console.log('📝 Opening updateEvent modal with item:', item);
      this.formData = {
        id: item.id,
        eventId: item.eventId,
        name: item.name,
        description: item.description || item.name,
        firstPrize: item.firstPrize || 0,
        secondPrize: item.secondPrize || 0,
        thirdPrize: item.thirdPrize || 0,
        startDate: item.startDate,
        endDate: item.endDate,
        status: item.status || 'Active'
      };
      console.log('📝 formData initialized:', this.formData);
    } else if (type === 'addEvent') {
      // Initialize event form with empty values
      this.formData = {
        eventId: '',
        name: '',
        description: '',
        firstPrize: 0,
        secondPrize: 0,
        thirdPrize: 0,
        startDate: '',
        endDate: ''
      };
    } else if (type === 'addProduct') {
      // Initialize product form with empty values
      this.formData = {
        productId: '',
        name: '',
        stock: 0,
        rewardPointsId: this.rewardPointsList.length > 0 ? this.rewardPointsList[0].id : ''
      };
    } else {
      this.formData = item ? { ...item } : {};
    }
    
    this.rejectReason = '';
    this.selectedUserAccount = null;
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.modalType = '';
    this.selectedItem = null;
    this.formData = {};
    this.rejectReason = '';
    this.selectedUserAccount = null;
  }

  // Action Handlers
  handleApprove(request: PendingRequest): void {
    if (!confirm(`Are you sure you want to approve the request for ${request.name}?`)) {
      return;
    }

    this.requestService.approveRequest(request.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          alert(`Request approved for ${request.name}`);
        },
        error: (error) => {
          console.error('Error approving request:', error);
          alert(error?.error?.message || 'Failed to approve request. Please try again.');
        }
      });
  }

  handleReject(): void {
    if (!this.rejectReason.trim()) {
      alert('Please provide a reason for rejection');
      return;
    }

    if (!this.selectedItem || !('id' in this.selectedItem)) {
      alert('Invalid request selected');
      return;
    }

    this.requestService.rejectRequest(this.selectedItem.id, this.rejectReason)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          alert(`Request rejected: ${this.rejectReason}`);
          this.closeModal();
        },
        error: (error) => {
          console.error('Error rejecting request:', error);
          alert(error?.error?.message || 'Failed to reject request. Please try again.');
        }
      });
  }

  // ✅ UPDATED: User status toggle not available until backend supports it
  toggleUserStatus(user: UserProfileDto): void {
    alert('User status toggle API not implemented yet in backend.');
  }

  // ✅ UPDATED: Save handler
  handleSave(): void {
    try {
      switch (this.modalType) {
        case 'updateProduct':
          if (this.selectedItem && 'id' in this.selectedItem) {
            // Get the selected reward point value
            const selectedRewardPoint = this.rewardPointsList.find(rp => rp.id === this.formData['rewardPointsId']);
            // Update the product via backend
            const updatedProduct: Product = {
              ...this.selectedItem,
              name: this.formData['name'] || this.selectedItem.name,
              stock: parseInt(this.formData['stock']) || this.selectedItem.stock,
              points: selectedRewardPoint?.pointsValue || this.selectedItem.points,
              status: this.formData['status'] || this.selectedItem.status,
            };
            this.productService.updateProduct(updatedProduct, this.formData['rewardPointsId']);
            this.closeModal();
          }
          break;

        case 'updateStock':
          if (this.selectedItem && 'id' in this.selectedItem) {
            this.productService.updateProductStock(
              this.selectedItem.id,
              parseInt(this.formData['stock']) || 0,
            );
            this.closeModal();
          }
          break;

        case 'updateRedeemPoint':
          if (this.selectedItem && 'id' in this.selectedItem) {
            this.redeemPointService.updateRedeemPointValue(
              this.selectedItem.id,
              parseInt(this.formData['pointsValue']) || 0,
            );
            this.closeModal();
          }
          break;

        case 'updateEvent':
          if (this.selectedItem && 'id' in this.selectedItem) {
            const updatedEvent = {
              ...this.selectedItem,
              ...this.formData,
            };
            this.eventService.updateEvent(updatedEvent)
              .pipe(takeUntil(this.destroy$))
              .subscribe({
                next: () => {
                  alert('Event updated successfully!');
                  this.closeModal();
                  // Reload events
                  this.eventService.getAllEvents()
                    .pipe(takeUntil(this.destroy$))
                    .subscribe();
                },
                error: (err) => {
                  console.error('Error updating event:', err);
                  alert(err?.error?.message || 'Failed to update event.');
                }
              });
            return;
          }
          break;

        // ✅ UPDATED: add user via backend API
        case 'addUser':
          this.createUserFromModal();
          return; // stop here, we close modal inside success

        case 'editUser':
          this.updateUserFromModal();
          return; // stop here, we close modal inside success

        case 'addEvent':
          console.log('📝 Form data FULL object:', this.formData);
          console.log('📝 Form data eventId:', this.formData.eventId);
          console.log('📝 Form data name:', this.formData.name);
          console.log('📝 Form data eventId LENGTH:', this.formData.eventId?.length);
          console.log('📝 Form data name LENGTH:', this.formData.name?.length);
          console.log('📝 Form data eventId truthy:', !!this.formData.eventId);
          console.log('📝 Form data name truthy:', !!this.formData.name);
          
          // Trim the values to remove any whitespace
          const eventId = (this.formData.eventId || '').trim();
          const name = (this.formData.name || '').trim();
          
          console.log('📝 After trim - eventId:', eventId);
          console.log('📝 After trim - name:', name);
          
          if (!eventId || !name) {
            console.error('❌ Validation failed - eventId:', eventId, 'name:', name);
            alert('Please fill in Code and Event Name. Code: "' + eventId + '", Name: "' + name + '"');
            return;
          }
          
          const newEvent: Event = {
            id: '00000000-0000-0000-0000-000000000000',
            eventId: eventId,
            name: name,
            description: (this.formData.description || '').trim(),
            firstPrize: parseInt(this.formData.firstPrize) || 0,
            secondPrize: parseInt(this.formData.secondPrize) || 0,
            thirdPrize: parseInt(this.formData.thirdPrize) || 0,
            startDate: this.formData.startDate || '',
            endDate: this.formData.endDate || '',
            status: EventStatus.Active,
          };
          console.log('📝 Event object being sent to service:', newEvent);
          
          this.eventService.addEvent(newEvent)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: () => {
                alert('Event created successfully!');
                this.closeModal();
              },
              error: (err) => {
                console.error('❌ Error creating event:', err);
                console.error('❌ Error status:', err.status);
                console.error('❌ Error response:', err.error);
                const errorMsg = err?.error?.message || err?.message || 'Failed to create event. Please check console for details.';
                alert(errorMsg);
              }
            });
          return;

        case 'addProduct':
          const selectedRewardPoint = this.rewardPointsList.find(rp => rp.id === this.formData['rewardPointsId']);
          const newProduct: Product = {
            id: '00000000-0000-0000-0000-000000000000',
            productId: this.formData['productId'] || '',
            name: this.formData['name'] || '',
            stock: parseInt(this.formData['stock']) || 0,
            points: selectedRewardPoint?.pointsValue || 0,
            status: ProductStatus.Active,
          };
          this.productService.addProduct(newProduct, this.formData['rewardPointsId']);
          this.closeModal();
          break;

        case 'addRedeemPoint':
          const pointsValue = parseInt(this.formData['pointsValue']) || 0;
          if (pointsValue > 0) {
            this.redeemPointService.addRedeemPoint(pointsValue);
            this.closeModal();
          } else {
            alert('Please enter a valid points value.');
          }
          break;

        default:
          console.warn(`Unknown modal type: ${this.modalType}`);
      }

      this.closeModal();
    } catch (error) {
      console.error('Error saving data:', error);
      alert('Failed to save. Please try again.');
    }
  }

  // ✅ Create user using backend API
  private createUserFromModal(): void {
    const payload = {
      employeeId: (this.formData['employeeId'] || '').trim(),
      email: (this.formData['email'] || '').trim().toLowerCase(),
      firstName: (this.formData['firstName'] || '').trim(),
      lastName: (this.formData['lastName'] || '').trim(),
      role: (this.formData['role'] || 'User').trim(),
      password: (this.formData['password'] || '').trim(),
    };

    if (
      !payload.employeeId ||
      !payload.email ||
      !payload.firstName ||
      !payload.lastName ||
      !payload.password
    ) {
      alert('Please fill all required fields.');
      return;
    }

    this.usersService
      .createUser(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (createdUser: UserProfileDto) => {
          alert(
            `User created: ${createdUser.firstName} ${createdUser.lastName}`,
          );
          // You can optionally push into local list
          this.users = [createdUser, ...this.users];
          this.stats.users = this.users.length;
          this.closeModal();
        },
        error: (err: any) => {
          console.error('Error creating user:', err);
          alert(err?.message || 'Failed to create user.');
        },
      });
  }

  // ✅ Update user using backend API
  private updateUserFromModal(): void {
    const userId = this.selectedItem?.id;
    
    if (!userId) {
      alert('Invalid user selected.');
      return;
    }

    const payload = {
      firstName: (this.formData['firstName'] || '').trim(),
      lastName: (this.formData['lastName'] || '').trim(),
      email: (this.formData['email'] || '').trim().toLowerCase(),
      role: (this.formData['role'] || '').trim(),
      accountStatus: this.formData['accountStatus'] || 'Active',
    };

    if (
      !payload.firstName ||
      !payload.lastName ||
      !payload.email ||
      !payload.role
    ) {
      alert('Please fill all required fields.');
      return;
    }

    console.log('🔄 Updating user:', userId, payload);

    this.usersService
      .updateUser(userId, payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updatedUser: UserProfileDto) => {
          console.log('✅ User updated:', updatedUser);
          alert(
            `User updated: ${updatedUser.firstName} ${updatedUser.lastName}`,
          );
          // Update the local list
          const index = this.users.findIndex(u => u.id === userId);
          if (index !== -1) {
            this.users[index] = updatedUser;
          }
          this.closeModal();
          // Reload users to get fresh data
          this.loadUsersData();
        },
        error: (err: any) => {
          console.error('❌ Error updating user:', err);
          alert(err?.error?.message || 'Failed to update user.');
        },
      });
  }

  // Helper method to reload users
  private loadUsersData(): void {
    this.usersService.getAllUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users: UserProfileDto[]) => {
          console.log('🔄 Reloaded users:', users);
          this.users = users;
          this.stats.users = users.length;
        },
        error: (err: any) => console.error('Error reloading users:', err),
      });
  }

  // ✅ Optional: open account info modal (if you want)
  openUserAccount(user: UserProfileDto): void {
    this.selectedUserAccount = null;

    this.usersService
      .getUserAccount(user.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (account: UserAccountDto) => {
          this.selectedUserAccount = account;
        },
        error: (err: any) => {
          console.error('Error loading user account:', err);
          alert('Failed to load account info.');
        },
      });
  }

  // Winners code unchanged
  openAssignWinnerPage(event: Event): void {
    try {
      this.modalType = 'assignWinner';
      this.selectedItem = event;
      this.formData = {
        eventInstanceId: event.id?.toString() || '',
        eventId: event.id?.toString() || '',
      };
      this.showModal = true;

      // Set eventRanks from the actual event prize values
      this.eventRanks = [
        { rank: 1, prizePoints: event.firstPrize || 0, medal: '🥇' },
        { rank: 2, prizePoints: event.secondPrize || 0, medal: '🥈' },
        { rank: 3, prizePoints: event.thirdPrize || 0, medal: '🥉' }
      ];
      console.log('🏆 Event ranks set from event:', this.eventRanks);

      this.loadParticipantsForEvent(event.id);
    } catch (error) {
      console.error('Error opening assign winner page:', error);
      alert('Failed to open assign winner dialog. Please try again.');
    }
  }

  private loadParticipantsForEvent(eventId: string): void {
    // Load actual users from the database instead of sample data
    this.usersService.getAllUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users) => {
          this.participants = users.map(user => ({
            id: user.id,
            name: `${user.firstName} ${user.lastName}`,
            email: user.email,
            currentPoints: user.account?.rewardBalance || 0,
          }));
          console.log('👥 Loaded participants from DB:', this.participants);
        },
        error: (err) => {
          console.error('❌ Error loading users for participants:', err);
          // Fallback to empty array
          this.participants = [];
        }
      });
  }

  hasWinnersSelected(): boolean {
    return this.eventRanks.some(
      (rank) => this.formData[`winners_${rank.rank}`],
    );
  }

  assignAllWinners(): void {
    try {
      // Get the selected winners for each rank
      const firstPlaceUserId = this.formData['winners_1'] || null;
      const secondPlaceUserId = this.formData['winners_2'] || null;
      const thirdPlaceUserId = this.formData['winners_3'] || null;

      if (!firstPlaceUserId && !secondPlaceUserId && !thirdPlaceUserId) {
        alert('Please select at least one winner.');
        return;
      }

      // Get the event ID (not the event instance ID)
      const eventId = this.selectedItem?.id;
      if (!eventId) {
        alert('No event selected.');
        return;
      }

      console.log('🏆 Assigning winners for event:', eventId);
      console.log('  1st Place:', firstPlaceUserId);
      console.log('  2nd Place:', secondPlaceUserId);
      console.log('  3rd Place:', thirdPlaceUserId);

      // Call the backend API to complete the event and assign winners
      this.eventService.completeEventWithWinners(
        eventId,
        firstPlaceUserId,
        secondPlaceUserId,
        thirdPlaceUserId
      ).pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            alert('Winners assigned successfully! Points have been awarded to their accounts.');
            this.closeModal();
            // Reload events to get updated data
            this.eventService.getAllEvents()
              .pipe(takeUntil(this.destroy$))
              .subscribe();
            // Reload users to show updated points
            this.usersService.getAllUsers()
              .pipe(takeUntil(this.destroy$))
              .subscribe();
          },
          error: (err) => {
            console.error('Error assigning winners:', err);
            alert(err?.error?.message || 'Failed to assign winners. Please make sure the event has prize values configured.');
          }
        });
    } catch (error) {
      console.error('Error assigning winners:', error);
      alert('Failed to assign winners. Please try again.');
    }
  }

  viewWinners(event: Event): void {
    // Show a simple alert with winner information
    // In a real implementation, this would open a modal showing winner details
    alert(`Winners have been assigned for "${event.name}".\n\nPrizes awarded:\n🥇 1st Place: ${event.firstPrize} points\n🥈 2nd Place: ${event.secondPrize} points\n🥉 3rd Place: ${event.thirdPrize} points\n\nCheck User Management tab to see updated point balances.`);
  }

  getParticipantName(participantId: string): string {
    const participant = this.participants.find((p) => p.id === participantId);
    return participant ? participant.name : 'Unknown';
  }

  clearWinner(rank: number): void {
    this.formData[`winners_${rank}`] = '';
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
    this.router.navigate(['/admin']);
  }

  navigateToProfile(): void {
    this.router.navigate(['/user/profile']);
  }
}
