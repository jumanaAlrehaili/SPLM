import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { RoleHelper } from '../services/role.helper';

export const adminGuard: CanActivateFn = () => {
  const router = inject(Router);
  const roleHelper = inject(RoleHelper);

  if (!localStorage.getItem('token')) {
    router.navigate(['/login']);
    return false;
  }

  if (roleHelper.isAdmin()) {
    return true;
  }

  // Authenticated but not an admin — send them to their dashboard home.
  router.navigate(['/dashboard/projects']);
  return false;
};
