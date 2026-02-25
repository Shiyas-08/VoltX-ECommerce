import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from 'src/environments';

export interface CartItem {
  id: number;
  productId: number;
  name: string;
  price: number;
  quantity: number;
  image?: string;
  category?: string;
    stock: number;           
  isOutOfStock?: boolean;
}

@Injectable({ providedIn: 'root' })
export class CartService {

  private api = environment.apiUrl + '/cart';

  private cartSubject = new BehaviorSubject<CartItem[]>([]);
  cart$ = this.cartSubject.asObservable();

// count navbar 
  cartCount$ = new BehaviorSubject<number>(0);

  constructor(private http: HttpClient) {}

  loadCart(): void {
    this.http.get<any>(this.api, { withCredentials: true }).subscribe({
      next: res => {
        const items = res.data ?? [];
        this.cartSubject.next(items);

      const count = items.length;

        this.cartCount$.next(count);
      },
      error: err => {
        console.error('Cart load failed', err);
        this.cartSubject.next([]);
        this.cartCount$.next(0);
      }
    });
  }
getCurrentCart(): CartItem[] {
  return this.cartSubject.value;
}

  getCartItems(): Observable<CartItem[]> {
    this.loadCart();
    return this.cart$;
  }

  addToCart(productId: number, quantity: number): Observable<any> {
    return this.http.post(
      this.api,
      { productId, quantity },
      { withCredentials: true }
    ).pipe(tap(() => this.loadCart()));
  }

  updateQuantity(productId: number, quantity: number): Observable<any> {
    return this.http.put(
      `${this.api}/update`,
      { productId, quantity },
      { withCredentials: true }
    ).pipe(tap(() => this.loadCart()));
  }

  removeFromCart(productId: number): Observable<any> {
    return this.http.delete(
      `${this.api}/${productId}`,
      { withCredentials: true }
    ).pipe(tap(() => this.loadCart()));
  }

  clearLocalCart(): void {
    this.cartSubject.next([]);
    this.cartCount$.next(0);
  }
}
