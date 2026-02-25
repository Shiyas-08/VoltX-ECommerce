import { Component } from '@angular/core';
import { AuthService } from 'src/app/core/services/auth.service';
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin',
  templateUrl: './dash-home.component.html',
  styleUrls: ['./dash-home.component.css'],
 
})
export class DashHomeComponent {
  constructor(private auth: AuthService, private router: Router) {}

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
