import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../../api/api/project.service';
import { ReleasePlanningComponent } from '../release-planning/release-planning.component';
import { Select } from 'primeng/select';
import { Button } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-release-planning-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReleasePlanningComponent, Select, ToastModule],
  templateUrl: './release-planning-page.component.html',
  styleUrl: './release-planning-page.component.scss'
})
export class ReleasePlanningPageComponent implements OnInit {
  private projectService = inject(ProjectService);
  private messageService = inject(MessageService);

  projects = signal<any[]>([]);
  selectedProjectId = signal<number | null>(null);
  loading = signal<boolean>(false);

  // Dropdown options for active project switching
  projectOptions: { label: string, value: number }[] = [];

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading.set(true);
    this.projectService.apiProjectGetMyProjectsGet().subscribe({
      next: (response) => {
        // Extract items from paginated response (or use raw array as fallback)
        const data = response?.items ?? response ?? [];
        // Pending=2, Member=3, Owner=4 — exclude None (1) only
        const memberProjects = (data || []).filter((p: any) => p.membershipStatusId >= 2);
        this.projects.set(memberProjects);
        
        this.projectOptions = memberProjects.map((p: any) => ({
          label: p.name,
          value: p.id
        }));

        // Auto-select the first project in the list if available
        if (memberProjects.length > 0) {
          this.selectedProjectId.set(memberProjects[0].id);
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load projects'
        });
        this.loading.set(false);
      }
    });
  }

  onProjectChange(projectId: number): void {
    this.selectedProjectId.set(projectId);
  }
}
