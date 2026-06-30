import { Component, Input, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProjectService, SetProjectLeadInput} from '../../../api';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select'; //dropdown 
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-project-leads',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, DialogModule, SelectModule, ButtonModule],
  templateUrl: './project-leads.component.html'
})
export class ProjectLeadsComponent implements OnInit {
  private projectService = inject(ProjectService);
  private messageService = inject(MessageService);

  @Input({ required: true }) projectId!: number;
  /** When false, the component skips loading leads/resources (user not part of the project). */
  @Input() canManage = false;

  visible = signal<boolean>(false); //dialog
  loading = signal<boolean>(false);

  // Data 
  leads = signal<any[]>([]);
  projectResources = signal<any[]>([]); // List of available members assigned to this project

  ngOnInit(): void {
    // Only members/owners may read leads; skip the calls otherwise to avoid 403 errors.
    if (!this.canManage) return;
    this.loadLeads();
    this.loadProjectResources();
  }

  loadLeads() {
    this.loading.set(true);
    this.projectService.getProjectLeads(this.projectId).subscribe({
      next: (data) => {
        this.leads.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load project leads' });
        this.loading.set(false);
      }
    });
  }

  // Fetch all resources assigned to this project for the dropdown selection
  loadProjectResources() {
    this.projectService.apiProjectGetProjectByIdIdGet(this.projectId).subscribe({
      next: (project: any) => {
        this.projectResources.set(project.resources || []);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load project resources' });
      }
    });
  }

  // Save or update the assigned leader immediately when dropdown value changes
  onLeadChange(leadRoleId: number, selectedUserId: number) {

    if (!selectedUserId) return;

    const input: SetProjectLeadInput = {
      leadRoleId: leadRoleId,
      userId: selectedUserId
    };

    this.projectService.assignProjectLeads(this.projectId, input).subscribe({
      next: () => { //
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Project lead updated successfully' });
        this.loadLeads(); // Re-fetch 
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'Unauthorized action';
        this.messageService.add({ severity: 'error', summary: 'Access Denied', detail: errorMsg });
        this.loadLeads();
      }
    });
  }

  // to trigger the dialog from the parent component
  showDialog() {
    this.visible.set(true);
  }
}