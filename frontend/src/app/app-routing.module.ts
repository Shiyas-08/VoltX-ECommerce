import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

const routes: Routes = [

  // ✅ ROOT redirect FIRST (VERY IMPORTANT)
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },

  // ✅ auth routes
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.module').then(m => m.AuthModule)
  },

  // ✅ main features
  {
    path: '',
    loadChildren: () =>
      import('./features/features.module').then(m => m.FeaturesModule)
  },

  // ✅ fallback
  { path: '**', redirectTo: 'home' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      scrollPositionRestoration: 'top'
    })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
