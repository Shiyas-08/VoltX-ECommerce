import { Injectable } from '@angular/core';
import {
  CanActivate,
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot
} from '@angular/router';
import { AuthService, User } from '../services/auth.service';
import { Observable } from 'rxjs';
import { filter, take, map } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {

  constructor(
    private auth: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  canActivate(): Observable<boolean> {
    return this.auth.authLoaded$.pipe(
      filter(v => v),
      take(1),
      map(() => {
        const user = this.auth.getUser();

        if (user && user.roleId === 1) return true;

        this.toastr.error('Admin access only');
        this.router.navigate([this.auth.getHomeRoute()]);
        return false;
      })
    );
  }
}
