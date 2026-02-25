import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ProductsRoutingModule } from './products-routing.module';

import { ProductListComponent } from './product-lists/product-lists.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { FeaturedProductsComponent } from './future-products/future-products.component';
import { ProductManagementComponent } from './product-management/product-management.component';
import { WisualListComponent } from './wishual-list/wishual-list.component';

@NgModule({
  declarations: [
    ProductListComponent,
    ProductDetailsComponent,
    FeaturedProductsComponent,
    ProductManagementComponent,
    WisualListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ProductsRoutingModule
  ],
  exports: [
    FeaturedProductsComponent   
  ]
})
export class ProductsModule {}
