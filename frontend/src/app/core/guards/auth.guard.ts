import { Injectable } from '@angular/core';
import {
  CanActivate,
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot
} from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable } from 'rxjs';
import { filter, take, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

canActivate(
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): Observable<boolean> {

  return this.auth.authLoaded$.pipe(
    filter(v => v),
    take(1),
    map(() => {

      if (this.auth.isLoggedIn()) return true;

      // ✅ save return url
      this.router.navigate(['/auth/login'], {
        queryParams: { returnUrl: state.url }
      });

      return false;
    })
  );
}
}
