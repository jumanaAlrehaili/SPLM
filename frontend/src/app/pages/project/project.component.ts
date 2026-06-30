import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TabsModule } from 'primeng/tabs';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { ToastModule } from 'primeng/toast';
import { DatePickerModule } from 'primeng/datepicker';
import { Select } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';

import { MessageService } from 'primeng/api';
import { ProjectService } from '../../api/api/project.service';
import { CreateProjectInput } from '../../api/model/createProjectInput';
import { SubmitJoinRequestInput } from '../../api/model/submitJoinRequestInput';
import { RoleHelper } from '../../services/role.helper';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-project',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    TableModule, ButtonModule, TabsModule, TagModule,
    DialogModule, InputTextModule, TextareaModule,
    TooltipModule, IconFieldModule, InputIconModule,
    ToastModule, DatePickerModule, Select, InputNumberModule
  ],
  templateUrl: './project.component.html',
  styleUrl: './project.component.scss'
})
export class ProjectComponent implements OnInit {
  // ── Data ──
  allProjects = signal<any[]>([]);
  myProjects = signal<any[]>([]);
  pendingRequests = signal<any[]>([]);
  myJoinRequests = signal<any[]>([]);

  // ── Pagination: All Projects ──
  allCurrentPage = 1;
  allPageSize = 10;
  allTotalRecords = 0;

  // ── Pagination: My Projects ──
  myCurrentPage = 1;
  myPageSize = 10;
  myTotalRecords = 0;

  // ── Pagination: Search ──
  searchCurrentPage = 1;
  searchPageSize = 10;
  searchTotalRecords = 0;
  isSearchMode = false;

  // ── UI state ──
  activeTab: string = '0';
  searchText = '';
  searchCreatedFrom: Date | null = null;
  searchCreatedTo: Date | null = null;
  showSearch = false;

  showCreateDialog = false;
  newProject: CreateProjectInput = { name: '', description: '' };

  durationUnitOptions = [
    { label: 'Days',   value: 1 },
    { label: 'Weeks',  value: 2 },
    { label: 'Months', value: 3 }
  ];

  showJoinDialog = false;
  joinProjectId: number | null = null;
  joinProjectName = '';
  joinReason = '';
  joinLoading = signal(false);

  // ── Role ──
  isPM = signal(false);

  constructor(
    private router: Router,
    private projectService: ProjectService,
    private roleHelper: RoleHelper,
    private messageService: MessageService,
    private cdr: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isPM.set(this.roleHelper.isPM());
    this.loadAllProjects(this.allCurrentPage, this.allPageSize);
    this.loadMyProjects(this.myCurrentPage, this.myPageSize);
    this.loadMyJoinRequests();
    if (this.isPM()) {
      this.loadPendingRequests();
    }
  }

  // ── Load Data ──

  loadAllProjects(page: number = 1, pageSize: number = this.allPageSize): void {
    this.projectService.apiProjectGetAllProjectsGet(page, pageSize).subscribe({
      next: (response) => {
        this.allProjects.set(response?.items ?? response ?? []);
        this.allTotalRecords = response?.totalCount ?? response?.length ?? 0;
        this.allCurrentPage = page;
      },
      error: (err) => console.error('Failed to load all projects', err)
    });
  }

  loadMyProjects(page: number = 1, pageSize: number = this.myPageSize): void {
    this.projectService.apiProjectGetMyProjectsGet(page, pageSize).subscribe({
      next: (response) => {
        this.myProjects.set(response?.items ?? response ?? []);
        this.myTotalRecords = response?.totalCount ?? response?.length ?? 0;
        this.myCurrentPage = page;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to load my projects', err)
    });
  }

  loadPendingRequests(): void {
    this.projectService.apiProjectGetPendingRequestsGet().subscribe({
      next: (data) => this.pendingRequests.set(data ?? []),
      error: (err) => console.error('Failed to load pending requests', err)
    });
  }

  loadMyJoinRequests(): void {
    this.projectService.apiProjectGetMyJoinRequestsGet().subscribe({
      next: (data) => this.myJoinRequests.set(data ?? []),
      error: (err) => console.error('Failed to load my join requests', err)
    });
  }

  // ── Lazy load handlers ──

  onAllProjectsPageChange(event: TableLazyLoadEvent): void {
    const first = event.first ?? 0;
    const rows = event.rows ?? this.allPageSize;
    const page = Math.floor(first / rows) + 1;
    this.allPageSize = rows;

    if (this.isSearchMode) {
      this.searchWithPagination(page, rows);
    } else {
      this.loadAllProjects(page, rows);
    }
  }

  onMyProjectsPageChange(event: TableLazyLoadEvent): void {
    const first = event.first ?? 0;
    const rows = event.rows ?? this.myPageSize;
    const page = Math.floor(first / rows) + 1;
    this.myPageSize = rows;
    this.loadMyProjects(page, rows);
  }

  get displayedAllTotalRecords(): number {
    return this.isSearchMode ? this.searchTotalRecords : this.allTotalRecords;
  }

  get displayedAllProjects(): any[] {
    return this.allProjects();
  }

  get isAllProjectsTab(): boolean {
    return this.activeTab === '0';
  }

  // ── Search ──

  search(): void {
    this.searchCurrentPage = 1;
    this.searchWithPagination(1, this.allPageSize);
  }

  private searchWithPagination(page: number, pageSize: number): void {
    const name = this.searchText.trim() || undefined;
    const createdFrom = this.searchCreatedFrom
      ? this.searchCreatedFrom.toISOString()
      : undefined;
    const createdTo = this.searchCreatedTo
      ? this.searchCreatedTo.toISOString()
      : undefined;

    this.projectService.apiProjectSearchProjectsGet(name, createdFrom, createdTo, page, pageSize).subscribe({
      next: (response) => {
        this.allProjects.set(response?.items ?? response ?? []);
        this.searchTotalRecords = response?.totalCount ?? response?.length ?? 0;
        this.searchCurrentPage = page;
        this.isSearchMode = true;
      },
      error: (err) => console.error('Search failed', err)
    });
  }

  toggleSearch(): void {
    this.showSearch = !this.showSearch;
    if (!this.showSearch) {
      this.clearSearch();
    }
  }

  clearSearch(): void {
    this.searchText = '';
    this.searchCreatedFrom = null;
    this.searchCreatedTo = null;
    this.isSearchMode = false;
    this.showSearch = false;
    this.allCurrentPage = 1;
    this.loadAllProjects(1, this.allPageSize);
  }


  // ── Project Actions ──

  viewProject(project: any): void {
    this.router.navigate(['/dashboard/projects', project.id]);
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

  openCreateDialog(): void {
    this.newProject = { name: '', description: '', budget: null, duration: null, durationUnit: null };
    this.showCreateDialog = true;
  }

  createProject(): void {
    this.projectService.apiProjectCreateNewProjectPost(this.newProject).subscribe({
      next: () => {
        this.showCreateDialog = false;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Project created successfully' });
        this.loadAllProjects(this.allCurrentPage, this.allPageSize);
        this.loadMyProjects(this.myCurrentPage, this.myPageSize);
      },
      error: (err) => {
        console.error('Failed to create project', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to create project' });
      }
    });
  }

  deleteProject(project: any): void {
    if (!confirm(`Are you sure you want to delete "${project.name}"?`)) return;

    this.projectService.apiProjectDeleteProjectIdDelete(project.id).subscribe({
      next: () => {
        this.allProjects.set(this.allProjects().filter((p: any) => p.id !== project.id));
        this.myProjects.set(this.myProjects().filter((p: any) => p.id !== project.id));
        this.messageService.add({ severity: 'success', summary: 'Deleted', detail: `"${project.name}" has been deleted` });
      },
      error: (err) => {
        console.error('Failed to delete project', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete project' });
      }
    });
  }

  // ── Join Request ──

  openJoinDialog(project: any): void {
    this.joinProjectId = project.id;
    this.joinProjectName = project.name;
    this.joinReason = '';
    this.joinLoading.set(false);
    this.showJoinDialog = true;
  }

  submitJoinRequest(): void {
    if (!this.joinProjectId) return;
    this.joinLoading.set(true);

    const input: SubmitJoinRequestInput = {
      projectId: this.joinProjectId,
      joinReason: this.joinReason || null
    };

    this.projectService.apiProjectSubmitNewJoinRequestPost(input).subscribe({
      next: () => {
        this.showJoinDialog = false;
        this.joinLoading.set(false);
        this.messageService.add({ severity: 'success', summary: 'Sent', detail: 'Join request submitted successfully' });
      },
      error: (err) => {
        this.joinLoading.set(false);
        console.error('Failed to submit join request', err);
        const detail = err.error?.message || err.error || 'Failed to submit join request';
        this.messageService.add({ severity: 'error', summary: 'Error', detail });
      }
    });
  }

  // ── Approve / Reject ──

  approveRequest(request: any): void {
    this.projectService.apiProjectApproveRequestIdPost(request.id).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Approved', detail: `Request from ${request.userName ?? 'user'} approved` });
        this.loadPendingRequests();
      },
      error: (err) => {
        console.error('Failed to approve request', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to approve request' });
      }
    });
  }

  rejectRequest(request: any): void {
    this.projectService.apiProjectRejectRequestIdPost(request.id).subscribe({
      next: () => {
        this.messageService.add({ severity: 'warn', summary: 'Rejected', detail: `Request from ${request.userName ?? 'user'} rejected` });
        this.loadPendingRequests();
      },
      error: (err) => {
        console.error('Failed to reject request', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to reject request' });
      }
    });
  }
}
