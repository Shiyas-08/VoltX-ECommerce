import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// USER 
import { NavbarComponent } from './components/navbar/navbar.component';
import { FooterComponent } from './components/footer/footer.component';
import { HomeComponent } from './components/home-page/home-page.component';

// ADMIN
import { AdminNavbarComponent } from './components/admin-nav/admin-nav.component';
import { DashHomeComponent } from './components/dashboard/dash-home/dash-home.component';
import { AdminDashboardComponent } from './components/dashboard/dashboard.component';

// Routing
import { SharedRoutingModule } from './shared-routing.module';
import { ProductsModule } from '../features/products/products.module';
import { UserPopupComponent } from './components/user-popup/user-popup.component';

@NgModule({
  declarations: [
    
    NavbarComponent,
    FooterComponent,
    HomeComponent,

   
    AdminNavbarComponent,
    DashHomeComponent,
    AdminDashboardComponent,
    UserPopupComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    SharedRoutingModule,
    ProductsModule
  ],
  exports: [
    NavbarComponent,
    FooterComponent,
    HomeComponent,

    AdminNavbarComponent,
    DashHomeComponent,
    AdminDashboardComponent
  ]
})
export class SharedModule {}
