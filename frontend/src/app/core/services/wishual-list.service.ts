import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from 'src/environments';

export interface WishlistItem {
  productId: number;
  productName: string;
  price: number;
  imageUrl?: string;
}

@Injectable({ providedIn: 'root' })
export class WishlistService {

  private api = environment.apiUrl + '/wishlist';
  private cartApi = environment.apiUrl + '/cart';

  private wishlistSubject = new BehaviorSubject<WishlistItem[]>([]);
  wishlist$ = this.wishlistSubject.asObservable();

  //count streams navbar
  count$ = new BehaviorSubject<number>(0);

  constructor(private http: HttpClient) {}

  //load
  loadWishlist(): void {
    this.http.get<any>(this.api, { withCredentials: true }).subscribe({
      next: res => {
        const items = res.data ?? [];
        this.wishlistSubject.next(items);
        this.count$.next(items.length);
      },
      error: err => {
        console.error(err);
        this.wishlistSubject.next([]);
        this.count$.next(0);
      }
    });
  }

  getWishlist(): Observable<WishlistItem[]> {
    this.loadWishlist();
    return this.wishlist$;
  }
// moveToCart(productId: number) {
//   return this.http.post<any>(
//     `${this.api}/cart${productId}`,
//     {},
//     { withCredentials: true }
//   );
// }
addToCart(productId: number) {
  return this.http.post(
    `${this.cartApi}`,
    { productId, quantity: 1 },
    { withCredentials: true }
  );
}


//add
  addToWishlist(productId: number): Observable<any> {
    return this.http.post(this.api, { productId }, { withCredentials: true }).pipe(
      tap(() => this.loadWishlist())
    );
  }
//remove 
  removeFromWishlist(productId: number): Observable<any> {
    return this.http.delete(`${this.api}/${productId}`, { withCredentials: true }).pipe(
      tap(() => this.loadWishlist())
    );
  }

  //helpers
  isInWishlist(productId: number): boolean {
    return this.wishlistSubject.value.some(
      item => item.productId === productId
    );
  }

  clearLocalWishlist(): void {
    this.wishlistSubject.next([]);
    this.count$.next(0);
  }
}

