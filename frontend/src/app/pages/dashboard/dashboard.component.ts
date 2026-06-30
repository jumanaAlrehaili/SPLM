import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { RouterOutlet, RouterModule, Router } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { AvatarModule } from 'primeng/avatar';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CommonModule } from '@angular/common';
import { SessionService } from '../../services/session.service';
import { NotificationService } from '../../services/notification.service';
import { RoleHelper } from '../../services/role.helper';
import { UserDto } from '../../api/model/userDto';
import { Observable } from 'rxjs';
import { PanelMenuModule } from 'primeng/panelmenu';
import { Popover } from 'primeng/popover';
import { ToastModule } from 'primeng/toast';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule, ButtonModule, MenuModule, TooltipModule, AvatarModule, PanelMenuModule, Popover, ToastModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  sidebarVisible = true;
  darkMode = false;

  currentUser$: Observable<UserDto | null>;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private router: Router,
    private sessionService: SessionService,
    public notificationService: NotificationService,
    private roleHelper: RoleHelper
  ) {
    this.currentUser$ = this.sessionService.currentUser$;
  }

  ngOnInit(): void {
    this.buildMenu();

    if (isPlatformBrowser(this.platformId)) {
      const saved = localStorage.getItem('theme');
      if (saved === 'dark') {
        this.darkMode = true;
        document.documentElement.classList.add('my-app-dark');
      } else if (saved === null) {
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        if (prefersDark) {
          this.darkMode = true;
          document.documentElement.classList.add('my-app-dark');
        }
      }
    }
  }

  toggleSidebar(): void {
    this.sidebarVisible = !this.sidebarVisible;
  }

  toggleTheme(): void {
    this.darkMode = !this.darkMode;
    if (this.darkMode) {
      document.documentElement.classList.add('my-app-dark');
      localStorage.setItem('theme', 'dark');
    } else {
      document.documentElement.classList.remove('my-app-dark');
      localStorage.setItem('theme', 'light');
    }
  }

  logout(): void {
    this.sessionService.logout();
    this.router.navigate(['/login']);
  }

  getInitials(username: string | null | undefined): string {
    if (!username) return 'U';
    return username.charAt(0).toUpperCase();
  }

  markAllNotificationsRead(): void {
    this.notificationService.markAllRead();
  }

  clearNotifications(): void {
    this.notificationService.clear();
  }

  notificationIcon(type: string | undefined): string {
    switch ((type ?? '').toLowerCase()) {
      case 'success': return 'pi pi-check-circle';
      case 'warning':
      case 'warn':    return 'pi pi-exclamation-triangle';
      case 'error':
      case 'danger':  return 'pi pi-times-circle';
      default:        return 'pi pi-info-circle';
    }
  }
  
  items: MenuItem[] = [
    {
      label: 'Main Menu',
      items: [
        { label: 'Projects',         icon: 'pi pi-folder',   routerLink: '/dashboard/projects' },
        {label: 'Project Releases', icon: 'pi pi-tags', items: [
          { label: 'Release Planning', icon: 'pi pi-calendar', routerLink: '/dashboard/release-planning' },
          { label: 'Release Stages',   icon: 'pi pi-sitemap',  routerLink: '/dashboard/release-stages' }
        ]},
        { label: 'Resources',        icon: 'pi pi-users',    routerLink: '/dashboard/resources' },

      ]
    }
  ];

  private buildMenu(): void {
    if (this.roleHelper.isAdmin()) {
      this.items = [
        ...this.items,
        {
          label: 'Administration',
          items: [
            { label: 'Holidays', icon: 'pi pi-calendar-times', routerLink: '/dashboard/admin/holidays' }
          ]
        }
      ];
    }
  }

  accountItems: MenuItem[] = [
    { label: 'Profile', icon: 'pi pi-user', routerLink: '/dashboard/profile' },
    { separator: true },
    { label: 'Logout', icon: 'pi pi-sign-out', command: () => this.logout() }
  ];
}
