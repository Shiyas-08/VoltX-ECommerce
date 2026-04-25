import { Component, OnInit } from '@angular/core';
import { WishlistService, WishlistItem } from 'src/app/core/services/wishual-list.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/core/services/auth.service';
import { CartService } from 'src/app/core/services/cart.service';

@Component({
  selector: 'app-visual-list',
  templateUrl: './wishual-list.component.html',
  styleUrls: ['./wishual-list.component.css']
})
export class WisualListComponent implements OnInit {

  wishlist: WishlistItem[] = [];

constructor(
  private wishlistService: WishlistService,
  private toastr: ToastrService,
  private router: Router,
  private auth: AuthService,
  private cartService: CartService 
) {}

  ngOnInit(): void {
    if (!this.auth.isLoggedIn()) {
      this.toastr.warning('Please login to view your wishlist');
      this.router.navigate(['/auth/login']);
      return;
    }

    // ✅ safer loading
    this.wishlistService.getWishlist().subscribe({
      next: items => this.wishlist = items,
      error: () => this.toastr.error('Failed to load wishlist')
    });
  }

  viewDetails(productId: number) {
    this.router.navigate(['/products/details', productId]);
  }

  // ✅ REMOVE FROM WISHLIST
  removeFromWishlist(productId: number, event?: Event) {
    event?.stopPropagation();

    this.wishlistService.removeFromWishlist(productId).subscribe({
      next: (res) => {
        this.toastr.success(res.message);

        // instant UI update
        this.wishlist = this.wishlist.filter(
          w => w.productId !== productId
        );
      },
      error: (err: any) => this.toastr.error(err.error?.message)
    });
  }
moveToCart(item: WishlistItem, event: Event) {
  event.stopPropagation();

  // optional stock check
  if ((item as any).stock === 0) {
    this.toastr.warning('Product is out of stock');
    return;
  }

  // ✅ USE CartService (IMPORTANT FIX)
  this.cartService.addToCart(item.productId, 1).subscribe({
    next: () => {

      // remove from wishlist
      this.wishlistService.removeFromWishlist(item.productId).subscribe({
        next: () => {
          this.toastr.success('Moved to cart');

          // instant UI update
          this.wishlist = this.wishlist.filter(
            w => w.productId !== item.productId
          );
        },
        error: (err: any) =>
          this.toastr.error(err.error?.message)
      });

    },
    error: (err: any) =>
      this.toastr.error(err.error?.message)
  });
}

}
