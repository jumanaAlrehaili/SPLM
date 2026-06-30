import { Routes } from '@angular/router';
import { ResetPasswordComponent } from './pages/auth/reset-password/reset-password';
import { ForgotPasswordComponent } from './pages/auth/forgot-password/forgot-password';
import { ProfileComponent } from './pages/dashboard/profile/profile.component';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  // ===== Public landing pages =====
  {
    path: '',
    loadComponent: () => import('./layouts/landing-layout/landing-layout.component').then(m => m.LandingLayoutComponent),
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent),
        pathMatch: 'full'
      },
      {
        path: 'about',
        loadComponent: () => import('./pages/about/about.component').then(m => m.AboutComponent)
      },
      {
        path: 'contact',
        loadComponent: () => import('./pages/contact/contact.component').then(m => m.ContactComponent)
      },
      {
        path: 'pricing',
        loadComponent: () => import('./pages/pricing/pricing.component').then(m => m.PricingComponent)
      }
    ]
  },

  // ===== Auth pages =====
  { path: 'login',          loadComponent: () => import('./pages/auth/login/login').then(m => m.Login) },
  { path: 'register',       loadComponent: () => import('./pages/auth/register/register.component').then(m => m.RegisterComponent) },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password',  component: ResetPasswordComponent },

  // ===== Protected dashboard =====
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    children: [
      { path: '', redirectTo: 'projects', pathMatch: 'full' },
      { path: 'projects', loadComponent: () => import('./pages/project/project.component').then(m => m.ProjectComponent) },
      { path: 'projects/:id', loadComponent: () => import('./pages/project/project-detail/project-detail.component').then(m => m.ProjectDetailComponent) },
      { path: 'projects/:projectId/features/:featureId', loadComponent: () => import('./pages/features/feature-detail/feature-detail.component').then(m => m.FeatureDetailComponent) },
      { path: 'release-planning', loadComponent: () => import('./pages/project/release-planning-page/release-planning-page.component').then(m => m.ReleasePlanningPageComponent) },
      { path: 'release-stages', loadComponent: () => import('./pages/project/release-stages-page/release-stages-page.component').then(m => m.ReleaseStagePage) },
      { path: 'resources', loadComponent: () => import('./pages/resources/resources-page/resources-page.component').then(m => m.ResourcesPageComponent) },
      { path: 'admin/holidays', canActivate: [adminGuard], loadComponent: () => import('./pages/admin/holidays/admin-holidays.component').then(m => m.AdminHolidaysComponent) },
      { path: 'profile', component: ProfileComponent }
    ]
  },

  { path: '**', redirectTo: '', pathMatch: 'full' }
];
