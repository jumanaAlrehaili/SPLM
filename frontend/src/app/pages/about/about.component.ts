import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AboutComponent {
  readonly values = [
    {
      title: 'Clarity over complexity',
      desc: 'We design every feature to eliminate ambiguity. A well-run product team should never wonder what is happening, who owns it, or why.'
    },
    {
      title: 'Accountability by default',
      desc: 'Ownership is visible. Every feature, every release, and every decision has a name and a timeline attached to it.'
    },
    {
      title: 'Speed without chaos',
      desc: 'Velocity matters. SPLM helps teams move fast by removing the coordination friction that slows delivery — without sacrificing visibility.'
    },
    {
      title: 'Built for real teams',
      desc: 'We build for teams who actually ship, not for perfect processes. Real constraints, real deadlines, real trade-offs — we get it.'
    }
  ];

  readonly stats = [
    { value: '10,000+', label: 'Projects managed' },
    { value: '500+',    label: 'Product teams' },
    { value: '98%',     label: 'Customer retention' },
    { value: '4.9/5',   label: 'Average rating' }
  ];
}
