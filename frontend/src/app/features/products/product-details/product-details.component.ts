import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from 'src/app/core/services/product.service';
import { CartService } from 'src/app/core/services/cart.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
})
export class ProductDetailsComponent implements OnInit {

  product: any;
  quantity = 1;
  selectedImage = '';

  constructor(
    private route: ActivatedRoute,
    private ps: ProductService,
    private cartService: CartService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) return;

    this.ps.getProductById(id).subscribe({
      next: res => {
        this.product = res.data ?? res;
        this.selectedImage = this.product?.imageUrls?.[0] || '';
      },
      error: () => this.toastr.error('Product not found')
    });
  }

  increaseQty() {
    if (this.quantity < this.product.stock) this.quantity++;
  }

  decreaseQty() {
    if (this.quantity > 1) this.quantity--;
  }

addToCart() {

  // ⭐ CHECK IF ALREADY IN CART
  const exists = this.cartService
    .getCurrentCart()
    .some(i => i.productId === this.product.id);

  if (exists) {
    this.toastr.info('Already in cart');
    return;
  }

  // ⭐ NORMAL ADD
  this.cartService.addToCart(this.product.id, this.quantity).subscribe({
    next: () => this.toastr.success('Added to cart'),
    error: (err: any) =>
      this.toastr.error(err.error?.message || 'Add to cart failed')
  });
}

  buyNow() {
    localStorage.setItem(
      'buyNowProduct',
      JSON.stringify({
        productId: this.product.id,
        name: this.product.name,
        price: this.product.price,
        image: this.selectedImage,
        quantity: this.quantity
      })
    );

    this.router.navigate(['/checkout'], {
      queryParams: { from: 'buy-now' }
    });
  }
}
