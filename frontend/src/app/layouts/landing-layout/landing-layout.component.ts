import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';

@Component({
  selector: 'app-landing-layout',
  standalone: true,
  imports: [RouterOutlet, RouterModule],
  templateUrl: './landing-layout.component.html',
  styleUrl: './landing-layout.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LandingLayoutComponent {
  readonly currentYear = new Date().getFullYear();
  readonly mobileMenuOpen = signal(false);

  toggleMobileMenu(): void {
    this.mobileMenuOpen.update(v => !v);
  }
}
