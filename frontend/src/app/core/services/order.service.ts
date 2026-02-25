import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private api = environment.apiUrl + '/orders';

  constructor(private http: HttpClient) {}

 //user

  placeOrder(formData: FormData) {
    return this.http.post<any>(`${this.api}/add-cart`, formData);
  }

  buyNow(formData: FormData) {
    return this.http.post<any>(`${this.api}/buy-now`, formData);
  }

  getMyOrders() {
    return this.http.get<any>(`${this.api}/my`);
  }

  cancelOrder(orderId: number) {
    return this.http.patch<any>(`${this.api}/${orderId}/cancel`, {});
  }

 
  //admin
  getAllOrders() {
    return this.http.get<any>(`${this.api}/admin/all`);
  }

  updateOrderStatus(orderId: number, status: string) {
    return this.http.patch<any>(
      `${this.api}/${orderId}/admin/status`,
      { status }
    );

    
  }

//payment

createPayment(orderId: number) {
  return this.http.post<any>(
    `${this.api}/${orderId}/create-payment`,
    {}
  );
}

verifyPayment(payload: {
  orderId: number;
  razorpayOrderId: string;
  razorpayPaymentId: string;
  razorpaySignature: string;
}) {
  return this.http.post<any>(
    `${this.api}/verify-payment`,
    payload
  );
}
getMyAddresses() {
  return this.http.get<any>(`${this.api}/my-addresses`);
}
}
