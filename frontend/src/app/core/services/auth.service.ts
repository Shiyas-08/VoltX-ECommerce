import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BehaviorSubject,
  catchError,
  of,
  switchMap,
  tap
} from 'rxjs';
import { environment } from 'src/environments';
import { Router } from '@angular/router';
export interface User {
  id: string;
  name: string;
  email: string;
  phone: string;
  roleId: number;
  isBlocked?: boolean;
}

export interface LegacyUser {
  id: string;
  name: string;
  email: string;
  role: 'user' | 'admin';
}


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private api = environment.apiUrl;


  private userSubject = new BehaviorSubject<User | null>(null);
  user$ = this.userSubject.asObservable();

  private authLoadedSubject = new BehaviorSubject<boolean>(false);
  authLoaded$ = this.authLoadedSubject.asObservable();

  constructor(
  private http: HttpClient,
  private router: Router
) {}



  login(data: { email: string; password: string }) {

    this.userSubject.next(null);

    return this.http.post(
      `${this.api}/auth/login`,
      data,
      { withCredentials: true }
    ).pipe(
      switchMap(() =>
        this.http.get<User>(
          `${this.api}/auth/profile`,
          { withCredentials: true }
        )
      ),
      tap(user => {
        this.userSubject.next(user);
        this.authLoadedSubject.next(true);
      })
    );
  }

//register
  register(data: any) {
    return this.http.post(
      `${this.api}/auth/register`,
      data,
      { withCredentials: true }
    );
  }
restoreSession() {
  const path = this.router.url;

  if (path.startsWith('/auth')) {
    this.authLoadedSubject.next(true);
    return;
  }

  this.refreshToken().pipe(
    switchMap(() =>
      this.http.get<User>(`${this.api}/auth/profile`, { withCredentials: true })
    ),
    catchError(() => of(null))
  ).subscribe(user => {
    this.userSubject.next(user);
    this.authLoadedSubject.next(true);
  });
}
getHomeRoute(): string {
  const user = this.userSubject.value;

  if (!user) return '/auth/login';
  if (user.roleId === 1) return '/admin/dashboard';
  return '/home';
}


logout() {
  return this.http.post(
    `${this.api}/auth/logout`,
    {},
    { withCredentials: true }
  ).pipe(
    tap(() => {
      this.userSubject.next(null);
      this.authLoadedSubject.next(false);

      localStorage.removeItem('lastRoute');
      localStorage.removeItem('currentUser');

      this.router.navigate(['/auth/login']);
    })
  );
}

//admin get all users
getAllUsers() {
  return this.http.get<User[]>(
    `${this.api}/auth/admin/users`,
    { withCredentials: true }
  );
}

//refresh token
  refreshToken() {
    return this.http.post(
      `${this.api}/auth/refresh`,
      {},
      { withCredentials: true }
    );
  }

//helpers
  isLoggedIn(): boolean {
    return this.userSubject.value !== null;
  }

  isAdmin(): boolean {
    return this.userSubject.value?.roleId === 1;
  }

  getUser(): User | null {
    return this.userSubject.value;
  }

  clearUser() {
    this.userSubject.next(null);
  }

  updateUser(user: User) {
    this.userSubject.next(user);
  }

  

  getUserId(): string | null {
    const raw = localStorage.getItem('currentUser');
    if (!raw) return null;

    try {
      const user = JSON.parse(raw);
      return user?.id ? String(user.id) : null;
    } catch {
      return null;
    }
  }

  // getCurrentUser(): LegacyUser | null {
  //   const raw = localStorage.getItem('currentUser');
  //   if (!raw) return null;

  //   try {
  //     return JSON.parse(raw);
  //   } catch {
  //     return null;
  //   }
  // }
  getCurrentUser(): User | null {
  return this.userSubject.value;
}


//password methods 
  forgotPassword(email: string) {
    return this.http.post(
      `${this.api}/auth/forgot-password`,
      { email }
    );
  }

  verifyOtp(email: string, otp: string) {
    return this.http.post(
      `${this.api}/auth/verify-otp`,
      { email, otp }
    );
  }

  resetPassword(email: string, newPassword: string) {
    return this.http.post(
      `${this.api}/auth/reset-password`,
      { email, newPassword }
    );
  }
}
