import { Injectable, Injector } from '@angular/core';
import {
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  private isRefreshing = false;
  private refreshTokenSubject = new BehaviorSubject<boolean | null>(null);

  constructor(private injector: Injector) {}

  private get auth(): AuthService {
    return this.injector.get(AuthService);
  }

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    // const publicRoutes = ['/home', '/products', '/about'];

    // Skip auth endpoints completely
    if (
      req.url.includes('/auth/login') ||
      req.url.includes('/auth/register') ||
      req.url.includes('/auth/refresh') ||
      req.url.includes('/auth/logout')
    ) {
      return next.handle(req);
    }

    return next.handle(req.clone({ withCredentials: true })).pipe(
      catchError((error: HttpErrorResponse) => {

        const msg =
          error?.error?.message?.toString()?.toUpperCase?.() || '';

        //  BLOCKED USER → FORCE LOGOUT (NO LOOP)
        if (msg.includes('USER_BLOCKED')) {

          // prevent redirect loop
          if (location.pathname !== '/auth/login') {
            this.auth.clearUser();
            window.location.href = '/auth/login';
          }

          return throwError(() => error);
        }

        //  not a 401 → pass through
        if (error.status !== 401) {
          return throwError(() => error);
        }

  
        // normal refresh flow
        return this.handle401(req, next);
      })
    );
  }

  private handle401(req: HttpRequest<any>, next: HttpHandler) {

    if (!this.auth.getCurrentUser?.()) {
      return throwError(() => new Error('No user session'));
    }

    if (this.isRefreshing) {
      return this.refreshTokenSubject.pipe(
        filter(v => v === true),
        take(1),
        switchMap(() =>
          next.handle(req.clone({ withCredentials: true }))
        )
      );
    }

    this.isRefreshing = true;
    this.refreshTokenSubject.next(null);

    return this.auth.refreshToken().pipe(
      switchMap(() => {
        this.isRefreshing = false;
        this.refreshTokenSubject.next(true);
        return next.handle(req.clone({ withCredentials: true }));
      }),
      catchError(err => {
        this.isRefreshing = false;

        //  refresh failed → logout once
        if (location.pathname !== '/auth/login') {
          this.auth.clearUser();
          window.location.href = '/auth/login';
        }

        return throwError(() => err);
      })
    );
  }
}




















// @Injectable()
// export class AuthInterceptor implements HttpInterceptor {

//   private isRefreshing = false;
//   private refreshTokenSubject = new BehaviorSubject<boolean | null>(null);

//   constructor(
//     private injector: Injector,
//     private router: Router
//   ) {}

//   private get auth(): AuthService {
//     return this.injector.get(AuthService);
//   }

//  intercept(req: HttpRequest<any>, next: HttpHandler) {

//   // Skip ALL auth bootstrap calls
//   if (
//     req.url.includes('/auth/login') ||
//     req.url.includes('/auth/register') ||
    
//     req.url.includes('/auth/refresh') ||
//     req.url.includes('/auth/logout') ||
//     location.pathname.startsWith('/auth')
//   ) {
//     return next.handle(req);
//   }

//   return next.handle(req.clone({ withCredentials: true })).pipe(
//     catchError((error: HttpErrorResponse) => {
//       if (error.status !== 401) {
//         return throwError(() => error);
//       }
//       return this.handle401(req, next);
//     })
//   );
// }


// private handle401(req: HttpRequest<any>, next: HttpHandler) {

//   //never refresh during logout
//   if (req.url.includes('/auth/logout')) {
//     this.forceLogout();
//     return throwError(() => new Error('Logout'));
//   }

//   if (!this.isRefreshing) {
//     this.isRefreshing = true;
//     this.refreshTokenSubject.next(null);

//     return this.auth.refreshToken().pipe(
//       switchMap(() => {
//         this.isRefreshing = false;
//         this.refreshTokenSubject.next(true);
//         return next.handle(req.clone({ withCredentials: true }));
//       }),
//       catchError(err => {
//         this.isRefreshing = false;
//         this.forceLogout();
//         return throwError(() => err);
//       })
//     );
//   }

//   return this.refreshTokenSubject.pipe(
//     filter(v => v === true),
//     take(1),
//     switchMap(() => next.handle(req.clone({ withCredentials: true })))
//   );
// }

//   private forceLogout() {
//     this.auth.clearUser();
//     this.router.navigate(['/auth/login']);
//   }
// }
