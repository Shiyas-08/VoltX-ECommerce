import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { OrdersRoutingModule } from './orders-routing.module';
import { MyOrdersComponent } from './my-orders/my-orders.component';
import { ManageOrdersComponent } from './order-management/order-management.component';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    MyOrdersComponent,
    ManageOrdersComponent
  ],
  imports: [
    CommonModule,
    OrdersRoutingModule,
    FormsModule
  ]
})
export class OrdersModule { }
