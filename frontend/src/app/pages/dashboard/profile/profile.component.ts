import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { AvatarModule } from 'primeng/avatar';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { SessionService } from '../../../services/session.service';
import { RoleHelper } from '../../../services/role.helper';
import { UserDto } from '../../../api/model/userDto';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, CardModule, ButtonModule, InputTextModule, AvatarModule, IconFieldModule, InputIconModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  currentUser$: Observable<UserDto | null>;

  constructor(
    private sessionService: SessionService,
    private roleHelper: RoleHelper
  ) {
    this.currentUser$ = this.sessionService.currentUser$;
  }

  ngOnInit(): void {}

  getUserRole(): string {
    return this.roleHelper.getUserRole() || 'User';
  }

  getInitials(username: string | null | undefined): string {
    if (!username) return 'U';
    return username.charAt(0).toUpperCase();
  }
}
