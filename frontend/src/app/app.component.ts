import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';
import { filter,take } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'E-commerce-project';

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}
ngOnInit(): void {

  // restore session
  this.authService.restoreSession();

  this.authService.authLoaded$
    .pipe(filter(v => v === true), take(1))
    .subscribe(() => {

      const user = this.authService.getUser();
      const currentUrl = this.router.url;

      // ⭐ KEY FIX — if admin is on user side, push to admin
      if (user?.roleId === 1 && !currentUrl.startsWith('/admin')) {
        this.router.navigate(['/admin/dashboard']);
        return;
      }

      // ⭐ guest opening root
      if (!user && (currentUrl === '/' || currentUrl === '')) {
        this.router.navigate(['/home']);
      }
    });

  // save last route
  this.router.events
    .pipe(filter(event => event instanceof NavigationEnd))
    .subscribe((event: any) => {

      const url = event.urlAfterRedirects;

      if (url.startsWith('/auth')) return;

      localStorage.setItem('lastRoute', url);
    });
}
}