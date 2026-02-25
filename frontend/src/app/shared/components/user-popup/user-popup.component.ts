import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-user-popup',
  templateUrl: './user-popup.component.html',
  styleUrls: ['./user-popup.component.css']
})
export class UserPopupComponent {

  @Input() user: any;

  @Output() closePopup = new EventEmitter<void>();
  @Output() profileEvent = new EventEmitter<void>();

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}


  get avatarLetter(): string {
    if (!this.user?.name) return '?';
    return this.user.name.charAt(0).toUpperCase();
  }

  close() {
    this.closePopup.emit();
  }

  goToProfile() {
    this.profileEvent.emit();
    this.close();
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
