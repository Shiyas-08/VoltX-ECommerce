import { Component, OnInit } from '@angular/core';
import { ProductService } from 'src/app/core/services/product.service';
import { CategoryService } from 'src/app/core/services/category.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CartService } from 'src/app/core/services/cart.service';
import { ToastrService } from 'ngx-toastr';
import { WishlistService } from 'src/app/core/services/wishual-list.service';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-lists.component.html',
})
export class ProductListComponent implements OnInit {

  products: any[] = [];
  categories: any[] = [];

  loading = false;

  searchTerm = '';
  categoryId: number | null = null;
  
  sortOption = 'priceLow'; 
//pagination 
  pageNumber = 1;
  pageSize = 6;
  totalCount = 0;

  constructor(
    private ps: ProductService,
    private cs: CategoryService,
    private router: Router,
    private route: ActivatedRoute,
    private cartService: CartService,
    private toastr: ToastrService,
    public wishlistService: WishlistService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();

   this.route.queryParams.subscribe(params => {
  if (params['q']) {
    this.searchTerm = params['q'];
    this.pageNumber = 1;
    this.loadProducts(); 
  }
});

  }

//load
  loadCategories() {
    this.cs.getAll().subscribe(res => {
      this.categories = res.data.filter((c: any) => c.isActive);
    });
  }

 loadProducts() {
  this.loading = true;

  const params: any = {
    pageNumber: this.pageNumber,
    pageSize: this.pageSize
  };

  if (this.searchTerm?.trim()) {
    params.search = this.searchTerm.trim();
  }

  if (this.categoryId) {
    params.categoryId = this.categoryId;
  }

  if (this.sortOption) {
    params.sort = this.sortOption;
  }

  this.ps.filterProducts(params).subscribe({
    next: res => {
      this.products = res.data.items;
      this.totalCount = res.data.totalCount;
      this.loading = false;
    },
    error: () => {
      this.loading = false;
    }
  });
}


//search
  searchProducts() {
    if (!this.searchTerm.trim()) {
      this.pageNumber = 1;
      this.loadProducts();
      return;
    }

    this.ps.searchProducts({
      search: this.searchTerm,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    }).subscribe(res => {
      this.products = res.data.items;
      this.totalCount = res.data.totalCount;
    });
  }

//filter
  applyFilters() {
    this.pageNumber = 1;
    this.loadProducts();
  }

  clearFilters() {
    this.searchTerm = '';
    this.categoryId = null;
    this.sortOption = '';
    this.pageNumber = 1;
    this.loadProducts();
  }

//pagination
  nextPage() {
    if (this.pageNumber * this.pageSize >= this.totalCount) return;
    this.pageNumber++;
    this.loadProducts();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  prevPage() {
    if (this.pageNumber === 1) return;
    this.pageNumber--;
    this.loadProducts();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }


  viewDetails(id: number) {
    this.router.navigate(['/products/details', id]);
  }

//  addToCart(product: any) {
//   if (!this.authService.isLoggedIn()) {
//     this.toastr.info('Login required to add items to cart');
//     return;
//   }

//   this.cartService.addToCart(product.id, 1).subscribe({
//     next: () => this.toastr.success('Added to cart'),
//     error: err => this.toastr.error(err.error?.message || 'Add to cart failed')
//   });
// }

addToCart(product: any) {
  if (!this.authService.isLoggedIn()) {
    this.toastr.info('Login required to add items to cart');
    return;
  }

  // ⭐ CHECK IF ALREADY IN CART (NEW)
  const exists = this.cartService
    .getCurrentCart()
    .some(i => i.productId === product.id);

  if (exists) {
    this.toastr.info('Already in cart');
    return;
  }

  // ⭐ NORMAL ADD
  this.cartService.addToCart(product.id, 1).subscribe({
    next: () => this.toastr.success('Added to cart'),
    error: (err: any) =>
      this.toastr.error(err.error?.message || 'Add to cart failed')
  });
}
toggleVisualList(product: any) {
  if (!this.authService.isLoggedIn()) {
    this.toastr.info('Login required to use wishlist');
    return;
  }

  const productId = product.id;

  if (this.wishlistService.isInWishlist(productId)) {
    this.wishlistService.removeFromWishlist(productId).subscribe({
      next: res => this.toastr.success(res.message),
      error: err => this.toastr.error(err.error?.message)
    });
  } else {
    this.wishlistService.addToWishlist(productId).subscribe({
      next: res => this.toastr.success(res.message),
      error: err => this.toastr.error(err.error?.message)
    });
  }
}


  }

