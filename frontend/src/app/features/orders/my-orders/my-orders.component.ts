import { Component, OnInit } from '@angular/core';
import { OrderService } from 'src/app/core/services/order.service';
import { ToastrService } from 'ngx-toastr';
import { Order } from 'src/app/core/models/order.model';

@Component({
  selector: 'app-my-orders',
  templateUrl: './my-orders.component.html'
})
export class MyOrdersComponent implements OnInit {

  orders: Order[] = [];
  //status tracking
  trackingSteps = ['Pending', 'Packed', 'Shipped', 'Delivered'];

  constructor(
    private orderService: OrderService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders() {
    this.orderService.getMyOrders().subscribe({
      next: res => {
        this.orders = res.data || [];
      },
      error: err => {
        this.toastr.error(err?.error?.message || 'Failed to load orders');
      }
    });
  }


getEffectiveStatus(order: Order): string {
  switch (order.status) {
    case 'Paid':
      return 'Packed';
    case 'OutForDelivery':
      return 'Shipped';
    default:
      return order.status;
  }
}

isStepCompleted(order: Order, step: string): boolean {
  const status = this.getEffectiveStatus(order);
  return this.trackingSteps.indexOf(status) >=
         this.trackingSteps.indexOf(step);
}


 getProgress(order: Order): number {
  const status = this.getEffectiveStatus(order);
  const index = this.trackingSteps.indexOf(status);
  if (index === -1) return 0;
  return ((index + 1) / this.trackingSteps.length) * 100;
}


getDisplayStatus(order: Order): string {
  if (order.status === 'Paid') return 'Order Confirmed';
  if (order.status === 'OutForDelivery') return 'Out for Delivery';
  return order.status;
}


  cancelOrder(order: Order) {
    if (order.status !== 'Pending') {
      this.toastr.warning('Order cannot be cancelled now');
      return;
    }

    this.orderService.cancelOrder(order.id!).subscribe({
      next: () => {
        order.status = 'Cancelled';
        this.toastr.success('Order cancelled successfully');
      },
      error: err => {
        this.toastr.error(err?.error?.message || 'Cancel failed');
      }
    });
  }
}
