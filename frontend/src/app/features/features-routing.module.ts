import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AdminGuard } from '../core/guards/admin.guard';
import { AuthGuard } from '../core/guards/auth.guard';

import { DashHomeComponent } from '../shared/components/dashboard/dash-home/dash-home.component';
import { AdminDashboardComponent } from '../shared/components/dashboard/dashboard.component';

import { ProductManagementComponent } from './products/product-management/product-management.component';
import { ManageOrdersComponent } from './orders/order-management/order-management.component';

const routes: Routes = [

  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },

  {
    path: 'about',
    loadChildren: () => import('../shared/about/about.module').then(m => m.AboutModule)
  },
  {
    path: 'products',
    loadChildren: () => import('./products/products.module').then(m => m.ProductsModule)
  },
  {
    path: 'cart',
    loadChildren: () => import('./cart/cart.module').then(m => m.CartModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'checkout',
    loadChildren: () => import('./checkout/checkout.module').then(m => m.CheckoutModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'orders',
    loadChildren: () => import('./orders/orders.module').then(m => m.OrdersModule),
    canActivate: [AuthGuard]
  },

  
  {
    path: 'admin',
    component: DashHomeComponent,
    canActivate: [AdminGuard],
    children: [

      
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },

      { path: 'dashboard', component: AdminDashboardComponent },
      { path: 'products', component: ProductManagementComponent },
      { path: 'orders', component: ManageOrdersComponent },

      {
        path: 'users',
        loadChildren: () => import('./users/users.module').then(m => m.UsersModule)
      },
      {
       path: 'categories', loadChildren: () =>import('./categories/categories.module').then(m => m.CategoriesModule)
}

    ]
  },
  {
  path: 'profile',
  loadChildren: () =>
    import('./profile/profile.module').then(m => m.ProfileModule),
  canActivate: [AuthGuard]
}

];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FeaturesRoutingModule {}
