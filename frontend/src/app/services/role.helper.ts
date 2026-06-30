import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RoleHelper {

  /**
   * Decodes the JWT payload from localStorage and returns the parsed claims.
   */
  private decodeToken(): any | null {
    const token = localStorage.getItem('token');
    if (!token) return null;

    try {
      const payload = token.split('.')[1];
      const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(decoded);
    } catch {
      return null;
    }
  }

  /**
   * Returns the user's role from the JWT, or null if not available.
   * Checks all common claim names used by .NET backends.
   */
  getUserRole(): string | null {
    const claims = this.decodeToken();
    if (!claims) return null;

    const role =
      claims.roleName ??
      claims.role ??
      claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      null;

    if (role === null) {
      console.warn('[RoleHelper] No role claim found. JWT keys:', Object.keys(claims));
    }

    return role;
  }

  /**
   * Returns the userId from the JWT, or null if not available.
   */
  getUserId(): number | null {
    const claims = this.decodeToken();
    const id = claims?.userId;
    return id ? Number(id) : null;
  }

  /**
   * Returns true if the current user has the "Admin" role.
   */
  isAdmin(): boolean {
    const role = (this.getUserRole() ?? '').trim().toLowerCase();
    return role === 'admin' || role === 'administrator';
  }

  /**
   * Returns true if the current user has the "Project Manager" role.
   */
  isPM(): boolean {
    return this.getUserRole() === 'Project Manager';
  }

  /**
   * Returns true if the current user has the "Stage Lead" role.
   */
  isStageLead(): boolean {
    return this.getUserRole() === 'Stage Lead';
  }

  /**
   * Returns true if the user can manage stages (PM or Stage Lead).
   */
  canManageStage(): boolean {
    return this.isPM() || this.isStageLead();
  }
}
