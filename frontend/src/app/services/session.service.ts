import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { AuthService } from '../api/api/auth.service';
import { UserDto } from '../api/model/userDto';
import { RegisterInputDto } from '../api/model/registerInputDto';
import { LoginInputDto } from '../api/model/loginInputDto';
import { ForgotPasswordDto } from '../api/model/forgotPasswordDto';
import { ResetPasswordDto } from '../api/model/resetPasswordDto';
import { LookupsService } from '../api/api/lookups.service';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

 private userState = new BehaviorSubject<UserDto | null>(null);

 currentUser$ = this.userState.asObservable();

 private notificationService = inject(NotificationService);
 private lookupsService = inject(LookupsService);

  constructor(private authService: AuthService) { }

  getRoles(): Observable<any> {
    return this.lookupsService.apiLookupsRolesGet();
  }

  register(data: RegisterInputDto): Observable<any> {
    return this.authService.apiAuthRegisterPost(data);
  }

  login(model: LoginInputDto): Observable<UserDto> {
    return this.authService.apiAuthLoginPost(model).pipe(
      map((response: UserDto) => {
        const user = response;

        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          if (user.token) {
            localStorage.setItem('token', user.token);
          }
          // Notify the entire app that a user has logged in (all subscribers of currentUser$).
          this.userState.next(user);
          // Open the real-time notifications channel.
          this.notificationService.start();
        }
        return user;
      })
    );
  }

  // Restores user data from localStorage into the BehaviorSubject during App Initialization (Refresh) to prevent session loss when memory resets.
  setCurrentUser(user: UserDto) {
    this.userState.next(user);
    // Re-open the notifications channel after a page refresh.
    this.notificationService.start();
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    // Broadcast null to indicate user is logged out
    this.userState.next(null);
    // Close the real-time notifications channel.
    this.notificationService.stop();
  }

  forgotPassword(dto: ForgotPasswordDto): Observable<any> {
    return this.authService.apiAuthForgotPasswordPost(dto);
  }

  resetPassword(dto: ResetPasswordDto): Observable<any> {
    return this.authService.apiAuthResetPasswordPost(dto);
  }
}
