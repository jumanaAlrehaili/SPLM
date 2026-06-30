import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { AvatarModule } from 'primeng/avatar';
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primeng/tabs';

import { FeatureListComponent } from '../../features/feature-list/feature-list.component';
import { ProjectService } from '../../../api/api/project.service';
import { ProjectLeadsComponent } from '../project-leads/project-leads.component';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, ButtonModule, DividerModule, AvatarModule, Tab,
    TabList, TabPanel, TabPanels, Tabs, FeatureListComponent, ProjectLeadsComponent],
  templateUrl: './project-detail.component.html',
  styleUrl: './project-detail.component.scss'
})
export class ProjectDetailComponent implements OnInit, OnDestroy {
  project = signal<any | null>(null);
  loading = signal(false);
  projectId = signal<number>(0); //to store and pass the projectID to the child component

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService
  ) { }

  ngOnInit(): void {
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const id = Number(params['id']);
      if (id) {
        this.projectId.set(id); //to update projectId signal
        this.loadProject(id);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProject(id: number): void {
    this.loading.set(true);
    this.projectService.apiProjectGetProjectByIdIdGet(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: (data) => {
        this.project.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load project', err);
        this.project.set(null);
        this.loading.set(false);
      }
    });
  }

  getDurationText(duration: number | null | undefined, unit: any): string {
    if (duration === null || duration === undefined) return '—';
    
    let resolvedUnit = unit;
    if (unit && typeof unit === 'object') {
      resolvedUnit = unit.value !== undefined ? unit.value : unit.label;
    }

    let label = '';
    const u = typeof resolvedUnit === 'string' ? resolvedUnit.trim().toLowerCase() : resolvedUnit;
    
    if (u === 1 || u === '1' || u === 'days' || u === 'day') {
      label = duration === 1 ? 'Day' : 'Days';
    } else if (u === 2 || u === '2' || u === 'weeks' || u === 'week') {
      label = duration === 1 ? 'Week' : 'Weeks';
    } else if (u === 3 || u === '3' || u === 'months' || u === 'month') {
      label = duration === 1 ? 'Month' : 'Months';
    } else {
      return `${duration}`;
    }
    
    return `${duration} ${label}`;
  }

  goBack(): void {
    this.router.navigate(['/dashboard/projects']);
  }
}
