import { Component, OnInit } from '@angular/core';
import { OrderService } from 'src/app/core/services/order.service';
import { ToastrService } from 'ngx-toastr';
import { Order, OrderStatus } from 'src/app/core/models/order.model';

@Component({
  selector: 'app-manage-orders',
  templateUrl: './order-management.component.html',
})
export class ManageOrdersComponent implements OnInit {

  orders: Order[] = [];
  loading = false;
  selectedOrder: Order | null = null;

  constructor(
    private orderService: OrderService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

//load orders
  loadOrders(): void {
    this.loading = true;

    this.orderService.getAllOrders().subscribe({
      next: res => {
        this.orders = (res.data || []).map((o: any) => ({
          ...o,
          status: this.normalizeStatus(o.status) // ✅ FIX
        }));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toastr.error('Failed to load orders');
      }
    });
  }

 
  normalizeStatus(status: any): OrderStatus {
    const map: Record<number, OrderStatus> = {
      1: 'Pending',
      2: 'Paid',
      3: 'Packed',
      4: 'Shipped',
      5: 'OutForDelivery',
      6: 'Delivered',
      99: 'Cancelled'
    };

    return typeof status === 'number' ? map[status] : status;
  }

 
  getNextStatuses(current: OrderStatus): OrderStatus[] {
    const flow: Record<OrderStatus, OrderStatus[]> = {
      Pending: ['Paid', 'Cancelled'],
      Paid: ['Packed', 'Cancelled'],
      Packed: ['Shipped'],
      Shipped: ['OutForDelivery'],
      OutForDelivery: ['Delivered'],
      Delivered: [],
      Cancelled: []
    };

    return flow[current] || []; 
  }

  
  changeStatus(order: Order, newStatus: OrderStatus): void {

    if (order.status === newStatus) return;

    this.orderService.updateOrderStatus(order.id, newStatus).subscribe({
      next: () => {
        order.status = newStatus;
        this.toastr.success('Order status updated');
      },
      error: err => {
        this.toastr.error(err.error?.message || 'Update failed');
      }
    });
  }

 
  viewOrder(order: Order): void {
    this.selectedOrder = order;
  }

  closeModal(): void {
    this.selectedOrder = null;
  }
}
