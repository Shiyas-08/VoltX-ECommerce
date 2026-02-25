
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router,NavigationEnd } from '@angular/router';
import { Subject, filter, takeUntil } from 'rxjs';

import { AuthService, User } from 'src/app/core/services/auth.service';
import { CartService } from 'src/app/core/services/cart.service';
import { WishlistService } from 'src/app/core/services/wishual-list.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit, OnDestroy {
// ✅ ADD THIS
isAuthPage = false;
  showUserPopup = false;
  showSearch = false;
  showDropdown = false;
  showMobileMenu = false;

  isLoggedIn = false;
  userData: User | null = null;
  isLoading = true;

  cartCount = 0;
  visualListCount = 0;

  searchQuery = '';

  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private router: Router,
    private cartService: CartService,
    private wishlistService: WishlistService
  ) {}

  ngOnInit(): void {
    this.listenToAuth();
    this.listenToCounts();
this.router.events
  .pipe(
    filter(event => event instanceof NavigationEnd),
    takeUntil(this.destroy$)
  )
  .subscribe(() => {
    this.isAuthPage = this.router.url.startsWith('/auth');
  });

// handle refresh
this.isAuthPage = this.router.url.startsWith('/auth');
    setTimeout(() => (this.isLoading = false), 300);

  }

listenToAuth() {
  this.authService.authLoaded$
    .pipe(
      filter(loaded => loaded === true),
      takeUntil(this.destroy$)
    )
    .subscribe(() => {
      const user = this.authService.getUser();

      this.userData = user;
      this.isLoggedIn = !!user;

      if (user) {
        this.cartService.loadCart();
        this.wishlistService.loadWishlist();
      } else {
        this.cartService.clearLocalCart();
        this.wishlistService.clearLocalWishlist();
      }

      this.isLoading = false;
    });
}

//   listenToAuth() {
//   this.authService.user$
//     .pipe(
//       filter(user => user !== undefined),
//       takeUntil(this.destroy$)
//     )
//     .subscribe(user => {
//       this.userData = user ?? null;
//       this.isLoggedIn = !!user;

//       if (user) {
//         this.cartService.loadCart();
//         this.wishlistService.loadWishlist();
//       } else {
//         this.cartService.clearLocalCart();
//         this.wishlistService.clearLocalWishlist();
//       }
//     });
// }




listenToCounts() {
  this.authService.authLoaded$
    .pipe(
      filter(v => v === true),
      takeUntil(this.destroy$)
    )
    .subscribe(() => {

      if (!this.isLoggedIn) {
        this.cartCount = 0;
        this.visualListCount = 0;
        return;
      }

      this.cartService.cartCount$
        .pipe(takeUntil(this.destroy$))
        .subscribe(count => this.cartCount = count);

      this.wishlistService.count$
        .pipe(takeUntil(this.destroy$))
        .subscribe(count => this.visualListCount = count);
    });
}


  openUserPopup() {
    this.showUserPopup = true;
  }

  closeUserPopup() {
    this.showUserPopup = false;
  }

  goToProfile() {
    this.closeUserPopup();
    this.router.navigate(['/profile']);
  }


  logout() {
    this.closeUserPopup();

    this.authService.logout().subscribe({
      next: () => {
        this.userData = null;
        this.isLoggedIn = false;
        this.router.navigate(['/auth/login']);
      },
      error: () => {
        this.userData = null;
        this.isLoggedIn = false;
        this.router.navigate(['/auth/login']);
      }
    });
  }

 

  onSearch() {
    const query = this.searchQuery.trim();
    if (!query) return;

    this.router.navigate(['/products'], {
      queryParams: { q: query }
    });

    this.searchQuery = '';
    this.showSearch = false;
  }

 

  toggleSearch() {
    this.showSearch = !this.showSearch;
  }

  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }

  toggleMobileMenu() {
    this.showMobileMenu = !this.showMobileMenu;
  }



  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
