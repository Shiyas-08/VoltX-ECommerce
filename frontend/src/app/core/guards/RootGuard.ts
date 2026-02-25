// import { Injectable } from '@angular/core';
// import { CanActivate, Router } from '@angular/router';
// import { AuthService } from '../services/auth.service';

// @Injectable({ providedIn: 'root' })
// export class RootGuard implements CanActivate {

//   constructor(
//     private auth: AuthService,
//     private router: Router
//   ) {}

//   canActivate(): boolean {
//     if (this.auth.isLoggedIn()) {
//       this.router.navigate(['/home']);
//     } else {
//       this.router.navigate(['/auth/login']);
//     }
//     return false; // stop current navigation
//   }
// }
