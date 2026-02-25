import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategoryManagementComponent } from './category-management/category-management.component';
import { AdminGuard } from 'src/app/core/guards/admin.guard';

const routes: Routes = [
  { path: '',
    component: CategoryManagementComponent,
    canActivate: [AdminGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CategoriesRoutingModule { }
