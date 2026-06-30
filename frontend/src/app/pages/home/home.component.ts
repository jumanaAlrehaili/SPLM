import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent {
  readonly features = [
    {
      icon: 'pi pi-folder-open',
      title: 'Project management',
      desc: 'Organize work into projects with clear ownership, milestones, and real-time progress tracking — all in one place.'
    },
    {
      icon: 'pi pi-list-check',
      title: 'Feature tracking',
      desc: 'Capture, prioritize, and move features through every stage of development — from backlog to shipped.'
    },
    {
      icon: 'pi pi-calendar',
      title: 'Release planning',
      desc: 'Coordinate releases with confidence. Map features to versions, manage dependencies, and ship on schedule.'
    },
    {
      icon: 'pi pi-users',
      title: 'Team collaboration',
      desc: 'Keep stakeholders aligned. Assign work, share updates, and resolve blockers before they slow you down.'
    }
  ];

  readonly steps = [
    {
      number: '01',
      title: 'Create a project',
      desc: 'Set up your project in minutes. Define the scope, invite your team, and configure access — no complex onboarding required.'
    },
    {
      number: '02',
      title: 'Plan your features',
      desc: 'Break the project down into features. Prioritize by business value, set owners, and link dependencies across teams.'
    },
    {
      number: '03',
      title: 'Ship your releases',
      desc: 'Assign features to releases, track completion status, and coordinate deployment with complete visibility from one dashboard.'
    }
  ];

  readonly enterpriseFeatures = [
    'Role-based access control',
    'Unlimited projects and features',
    'Release pipeline management',
    'Team performance analytics',
    'Audit trails and compliance reporting',
    'REST API access and webhooks'
  ];
}
