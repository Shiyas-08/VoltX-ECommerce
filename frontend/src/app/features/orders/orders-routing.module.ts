  import { NgModule } from '@angular/core';
  import { RouterModule, Routes } from '@angular/router';
  import { MyOrdersComponent } from './my-orders/my-orders.component';
  import { ManageOrdersComponent } from './order-management/order-management.component';

  const routes: Routes = [{ path: '', component: MyOrdersComponent }

  ];

  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
  })
  export class OrdersRoutingModule { }
