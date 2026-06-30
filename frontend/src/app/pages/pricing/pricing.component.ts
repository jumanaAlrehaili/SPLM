import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import { RouterModule } from '@angular/router';

interface FaqItem {
  question: string;
  answer: string;
  open: boolean;
}

@Component({
  selector: 'app-pricing',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './pricing.component.html',
  styleUrl: './pricing.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PricingComponent {
  readonly annual = signal(false);

  toggleBilling(): void {
    this.annual.update(v => !v);
  }

  readonly faqs: FaqItem[] = [
    {
      question: 'Can I change plans at any time?',
      answer: 'Yes. You can upgrade, downgrade, or cancel at any time. Upgrades take effect immediately; downgrades apply at the next billing cycle.',
      open: false
    },
    {
      question: 'What happens when I reach the project limit on the Starter plan?',
      answer: 'You will be prompted to upgrade to Professional. All existing projects and data remain fully accessible.',
      open: false
    },
    {
      question: 'Is there a free trial of the Professional plan?',
      answer: 'Yes. All Professional plan features are available free for 14 days. No credit card required to start.',
      open: false
    },
    {
      question: 'Do you offer discounts for non-profit or educational organizations?',
      answer: 'Yes — contact us at hello@splm.io with your institutional email address for eligibility and discount details.',
      open: false
    },
    {
      question: 'How does per-user pricing work?',
      answer: 'You are billed for each active user on your team per month. Inactive or deactivated users are not counted.',
      open: false
    }
  ];

  toggleFaq(index: number): void {
    this.faqs[index].open = !this.faqs[index].open;
  }
}
