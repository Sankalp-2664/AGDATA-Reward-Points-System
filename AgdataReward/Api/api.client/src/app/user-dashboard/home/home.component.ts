import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  constructor(private router: Router) {}

  ngOnInit(): void {
    // Blank for now
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
}
