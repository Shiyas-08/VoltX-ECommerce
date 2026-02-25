// import { Component, OnInit } from '@angular/core';
// import { ActivatedRoute, Router } from '@angular/router';
// import { CartService } from 'src/app/core/services/cart.service';
// import { OrderService } from 'src/app/core/services/order.service';
// import { ToastrService } from 'ngx-toastr';

// @Component({
//   selector: 'app-checkout',
//   templateUrl: './checkout.component.html',
// })
// export class CheckoutComponent implements OnInit {

//   checkoutItems: any[] = [];
//   fromBuyNow = false;
//   loading = false;

//   // 🔒 Payment placeholder (Razorpay later)
//   paymentMethod: 'UPI' | 'CARD' = 'UPI';

//   addressModel = {
//     fullName: '',
//     phone: '',
//     addressLine1: '',
//     addressLine2: '',
//     city: '',
//     state: '',
//     pincode: ''
//   };

//   constructor(
//     private route: ActivatedRoute,
//     private cartService: CartService,
//     private orderService: OrderService,
//     private toastr: ToastrService,
//     private router: Router
//   ) {}

//   ngOnInit(): void {
//     const from = this.route.snapshot.queryParamMap.get('from');
//     this.fromBuyNow = from === 'buy-now';

//     if (this.fromBuyNow) {
//       const raw = localStorage.getItem('buyNowProduct');
//       if (raw) {
//         const p = JSON.parse(raw);
//         p.quantity = p.quantity || 1;
//         this.checkoutItems = [p];
//       }
//     } else {
//       this.cartService.getCartItems().subscribe(items => {
//         this.checkoutItems = items.map(i => ({
//           ...i,
//           quantity: i.quantity || 1
//         }));
//       });
//     }
//   }

//   getTotal(): number {
//     return this.checkoutItems.reduce(
//       (sum, item) => sum + item.price * item.quantity,
//       0
//     );
//   }

// placeOrder() {
//   if (this.loading) return;

//   const a = this.addressModel;

//   if (
//     !a.fullName.trim() ||
//     !a.phone.trim() ||
//     !a.addressLine1.trim() ||
//     !a.city.trim() ||
//     !a.state.trim() ||
//     !a.pincode.trim()
//   ) {
//     this.toastr.warning('Please fill all address details');
//     return;
//   }

//   if (this.checkoutItems.length === 0) {
//     this.toastr.warning('No items to checkout');
//     return;
//   }

//   const fd = new FormData();
//   fd.append('FullName', a.fullName.trim());
//   fd.append('Phone', a.phone.trim());
//   fd.append('AddressLine1', a.addressLine1.trim());
//   fd.append('AddressLine2', a.addressLine2?.trim() || '');
//   fd.append('City', a.city.trim());
//   fd.append('State', a.state.trim());
//   fd.append('Pincode', a.pincode.trim());

//  // ✅ VERY IMPORTANT PART (BUY NOW FIX)
// if (this.fromBuyNow) {
//   const raw = localStorage.getItem('buyNowProduct');
//   if (!raw) {
//     this.toastr.error('Buy now product missing');
//     return;
//   }

//   const item = JSON.parse(raw);

//   fd.append('ProductId', item.productId.toString());
//   fd.append('Quantity', item.quantity.toString());
// }

//   this.loading = true;

//   const request$ = this.fromBuyNow
//     ? this.orderService.buyNow(fd)
//     : this.orderService.placeOrder(fd);

//   request$.subscribe({
//     next: res => {
//       this.toastr.success(res.message || 'Order placed successfully');

//       if (this.fromBuyNow) {
//         localStorage.removeItem('buyNowProduct');
//       }

//       this.loading = false;
//       this.router.navigate(['/orders']);
//     },
//     error: err => {
//       this.loading = false;
//       this.toastr.error(err.error?.message || 'Failed to place order');
//     }
//   });
// }

//   }

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CartService } from 'src/app/core/services/cart.service';
import { OrderService } from 'src/app/core/services/order.service';
import { ToastrService } from 'ngx-toastr';

declare var Razorpay: any;

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
})
export class CheckoutComponent implements OnInit {

  checkoutItems: any[] = [];
  fromBuyNow = false;
  loading = false;
  savedAddresses: any[] = [];
selectedAddressId: number | null = null;
selectedProductIds: number[] = [];
  paymentMethod: 'UPI' | 'CARD' = 'UPI';
  saveAddress = false;
  addressModel = {
    fullName: '',
    phone: '',
    addressLine1: '',
    addressLine2: '',
    city: '',
    state: '',
    pincode: ''
  };
  states: string[] = [
  'Kerala',
  'Tamil Nadu',
  'Karnataka',
  'Maharashtra',
  'Delhi',
  'Telangana',
  'Andhra Pradesh',
  'Gujarat',
  'Rajasthan',
  'Uttar Pradesh'
];
  constructor(
    private route: ActivatedRoute,
    private cartService: CartService,
    private orderService: OrderService,
    private toastr: ToastrService,
    private router: Router
  ) {}
ngOnInit(): void {
  const from = this.route.snapshot.queryParamMap.get('from');
  this.fromBuyNow = from === 'buy-now';

  // 🔹 READ SELECTED PRODUCTS FROM CART PAGE
  const navigation = this.router.getCurrentNavigation();
const state =
  navigation?.extras?.state ||
  history.state;

if (state?.selectedProductIds?.length) {
  this.selectedProductIds = state.selectedProductIds;
}

  // 🔹 BUY NOW FLOW (UNCHANGED)
  if (this.fromBuyNow) {
    const raw = localStorage.getItem('buyNowProduct');
    if (raw) {
      const p = JSON.parse(raw);
      p.quantity = p.quantity || 1;
      this.checkoutItems = [p];
    }
  }

  // 🔹 CART FLOW (FILTER SELECTED ITEMS ONLY)
  else {
    // 🚫 Prevent direct checkout without selection
    if (!this.selectedProductIds.length) {
      this.router.navigate(['/cart']);
      return;
    }

    this.cartService.getCartItems().subscribe(items => {
      this.checkoutItems = items
        .filter(item =>
          this.selectedProductIds.includes(item.productId)
        )
        .map(i => ({
          ...i,
          quantity: i.quantity || 1
        }));
    });
  }

  this.loadSavedAddresses();
}
allowOnlyNumbers(event: KeyboardEvent) {
  const charCode = event.which ? event.which : event.keyCode;
  if (charCode < 48 || charCode > 57) {
    event.preventDefault();
  }
}
  getTotal(): number {
    return this.checkoutItems.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0
    );
  }
  useNewAddress() {
  this.selectedAddressId = null;

  this.addressModel = {
    fullName: '',
    phone: '',
    addressLine1: '',
    addressLine2: '',
    city: '',
    state: '',
    pincode: ''
  };
}
selectSavedAddress(addr: any) {
  this.selectedAddressId = addr.id;

  this.addressModel = {
    fullName: addr.fullName || '',
    phone: addr.phone || '',
    addressLine1: addr.addressLine1 || '',
    addressLine2: addr.addressLine2 || '',
    city: addr.city || '',
    state: addr.state || '',
    pincode: addr.pincode || ''
  };
}
  placeOrder() {
    if (this.loading) return;

    const a = this.addressModel;

    if (
      !a.fullName.trim() ||
      !a.phone.trim() ||
      !a.addressLine1.trim() ||
      !a.city.trim() ||
      !a.state.trim() ||
      !a.pincode.trim()
    ) {
      this.toastr.warning('Please fill all address details');
      return;
    }

    if (this.checkoutItems.length === 0) {
      this.toastr.warning('No items to checkout');
      return;
      
    }

    const fd = new FormData();
    fd.append('FullName', a.fullName.trim());
    fd.append('Phone', a.phone.trim());
    fd.append('AddressLine1', a.addressLine1.trim());
    fd.append('AddressLine2', a.addressLine2?.trim() || '');
    fd.append('City', a.city.trim());
    fd.append('State', a.state.trim());
    fd.append('Pincode', a.pincode.trim());
    fd.append('SaveAddress', this.saveAddress ? 'true' : 'false');

    // BUY NOW SUPPORT
    if (this.fromBuyNow) {
      const raw = localStorage.getItem('buyNowProduct');
      if (!raw) {
        this.toastr.error('Buy now product missing');
        return;
      }

      const item = JSON.parse(raw);
      fd.append('ProductId', item.productId.toString());
      fd.append('Quantity', item.quantity.toString());
    }
if (!this.fromBuyNow) {
  this.selectedProductIds.forEach(id => {
    fd.append('CartProductIds', id.toString());
  });
}
    this.loading = true;

    const request$ = this.fromBuyNow
      ? this.orderService.buyNow(fd)
      : this.orderService.placeOrder(fd);

    request$.subscribe({
      next: res => {
        const orderId = res.data;

        if (!orderId) {
          this.loading = false;
          this.toastr.error('Order ID missing');
          return;
        }

        if (this.fromBuyNow) {
          localStorage.removeItem('buyNowProduct');
        }

        // 🔥 START PAYMENT AFTER ORDER CREATION
        this.startPayment(orderId);
      },
      error: err => {
        this.loading = false;
        this.toastr.error(err.error?.message || 'Failed to place order');
      }
    });
  }

  startPayment(orderId: number) {
    this.orderService.createPayment(orderId).subscribe({
      next: res => {
        this.openRazorpay(orderId, res.data);
      },
      error: err => {
        this.loading = false;
        this.toastr.error(err.error?.message || 'Payment initiation failed');
      }
    });
  }

  // 🔥 OPEN RAZORPAY CHECKOUT
  openRazorpay(orderId: number, data: any) {
    const options = {
      key: data.key,
      amount: data.amount * 100,
      currency: 'INR',
      name: 'E-Commerce',
      description: 'Order Payment',
      order_id: data.razorpayOrderId,

      handler: (response: any) => {
        this.verifyPayment(orderId, response);
      },

      modal: {
        ondismiss: () => {
          this.loading = false;
          this.toastr.info('Payment cancelled');
        }
      }
    };

    const rzp = new Razorpay(options);
    rzp.open();
  }

  // 🔥 VERIFY PAYMENT (BACKEND)
  verifyPayment(orderId: number, response: any) {
    const payload = {
      orderId: orderId,
      razorpayOrderId: response.razorpay_order_id,
      razorpayPaymentId: response.razorpay_payment_id,
      razorpaySignature: response.razorpay_signature
    };

    this.orderService.verifyPayment(payload).subscribe({
      next: () => {
        this.loading = false;
        this.toastr.success('Payment successful');
        this.router.navigate(['/orders']);
      },
      error: err => {
        this.loading = false;
        this.toastr.error(err.error?.message || 'Payment verification failed');
      }
    });
  }
  loadSavedAddresses() {
  this.orderService.getMyAddresses().subscribe({
    next: res => {
      this.savedAddresses = res.data || [];
    },
    error: () => {
      this.savedAddresses = [];
    }
  });
}

}
