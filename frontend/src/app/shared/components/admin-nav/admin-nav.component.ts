import { Component, HostListener, OnInit } from '@angular/core';
import { AuthService, User } from 'src/app/core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-navbar',
  templateUrl: './admin-nav.component.html',
  styleUrls: ['./admin-nav.component.css']
})
export class AdminNavbarComponent implements OnInit {

  adminName = '';
  adminEmail = '';
  dropdownOpen = false;

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // ✅ ALWAYS use live auth state
    const user = this.auth.getUser();

    if (user && user.roleId === 1) {
      this.adminName = user.name || 'Admin';
      this.adminEmail = user.email || '';
    }
  }

  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }
@HostListener('document:click')
closeDropdown(): void {
  this.dropdownOpen = false;
}



 onLogout() {
    this.auth.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: () => {
        // Even if backend fails, go to login
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
