// import { Component, OnInit } from '@angular/core';
// import { CartService, CartItem } from 'src/app/core/services/cart.service';
// import { ToastrService } from 'ngx-toastr';
// import { Router } from '@angular/router';

// @Component({
//   selector: 'app-cart',
//   templateUrl: './cart.component.html',
//   styleUrls: ['./cart.component.css']
// })
// export class CartComponent implements OnInit {
//   cartItems: CartItem[] = [];
//   total = 0;

//   constructor(
//     private cartService: CartService,
//     private toastr: ToastrService,
//     private router: Router
//   ) {}

// ngOnInit(): void {
//   this.cartService.getCartItems().subscribe(items => {
//     this.cartItems = items.map(item => ({
//       ...item,
//       isOutOfStock: item.stock <= 0
//     }));

//     this.calculateTotal();
//   });
// }


//   calculateTotal(): void {
//     this.total = this.cartItems.reduce(
//       (sum, item) => sum + item.price * item.quantity,
//       0
//     );
//   }

//   increaseQty(item: CartItem) {
//     this.cartService
//       .updateQuantity(item.productId, item.quantity + 1)
//       .subscribe({
//         next: res => this.toastr.success(res.message),
//         error: err =>
//           this.toastr.error(err.error?.message || 'Insufficient stock')
//       });
//   }

//   decreaseQty(item: CartItem) {
//     if (item.quantity <= 1) return;

//     this.cartService
//       .updateQuantity(item.productId, item.quantity - 1)
//       .subscribe({
//         next: res => this.toastr.success(res.message),
//         error: err =>
//           this.toastr.error(err.error?.message)
//       });
//   }

//   removeItem(item: CartItem) {
//     this.cartService.removeFromCart(item.productId).subscribe({
//       next: res => this.toastr.success(res.message),
//       error: err => this.toastr.error(err.error?.message)
//     });
//   }

//   goToCheckout() {
//     if (this.cartItems.length === 0) {
//       this.toastr.warning('Your cart is empty!', 'Cannot Checkout');
//       return;
//     }

//     this.router.navigate(['/checkout'], {
//       queryParams: { from: 'cart' }
//     });
//   }
// }
import { Component, OnInit } from '@angular/core';
import { CartService, CartItem } from 'src/app/core/services/cart.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  total = 0;

  // ✅ NEW STATE (safe)
  selectedProductIds: number[] = [];
  selectedTotal = 0;

  constructor(
    private cartService: CartService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cartService.getCartItems().subscribe(items => {
      this.cartItems = items.map(item => ({
        ...item,
        isOutOfStock: item.stock <= 0
      }));

      this.calculateTotal();
      this.syncSelections();
      this.calculateSelectedTotal();
    });
  }

  calculateTotal(): void {
    this.total = this.cartItems.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0
    );
  }

  // ✅ NEW METHODS (do not affect existing ones)
  toggleSelection(productId: number): void {
    if (this.selectedProductIds.includes(productId)) {
      this.selectedProductIds = this.selectedProductIds.filter(id => id !== productId);
    } else {
      this.selectedProductIds.push(productId);
    }
    this.calculateSelectedTotal();
  }

  calculateSelectedTotal(): void {
    this.selectedTotal = this.cartItems
      .filter(item => this.selectedProductIds.includes(item.productId))
      .reduce((sum, item) => sum + item.price * item.quantity, 0);
  }

  syncSelections(): void {
    this.selectedProductIds = this.selectedProductIds.filter(id =>
      this.cartItems.some(item => item.productId === id)
    );
  }

  // 🔒 EXISTING METHODS (unchanged)
  increaseQty(item: CartItem) {
    this.cartService
      .updateQuantity(item.productId, item.quantity + 1)
      .subscribe({
        next: res => this.toastr.success(res.message),
        error: err =>
          this.toastr.error(err.error?.message || 'Insufficient stock')
      });
  }

  decreaseQty(item: CartItem) {
    if (item.quantity <= 1) return;

    this.cartService
      .updateQuantity(item.productId, item.quantity - 1)
      .subscribe({
        next: res => this.toastr.success(res.message),
        error: err => this.toastr.error(err.error?.message)
      });
  }

  removeItem(item: CartItem) {
    this.cartService.removeFromCart(item.productId).subscribe({
      next: res => this.toastr.success(res.message),
      error: err => this.toastr.error(err.error?.message)
    });
  }

  goToCheckout() {
    if (this.selectedProductIds.length === 0) {
      this.toastr.warning('Select at least one item to checkout');
      return;
    }

    this.router.navigate(['/checkout'], {
      state: {
        selectedProductIds: this.selectedProductIds
      }
    });
  }
}