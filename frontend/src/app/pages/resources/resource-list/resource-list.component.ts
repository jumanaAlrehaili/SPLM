import { Component, Input, OnInit, OnDestroy, ViewChild, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

import { Table, TableModule, TableLazyLoadEvent } from 'primeng/table';
import { Tag } from 'primeng/tag';
import { MultiSelect } from 'primeng/multiselect';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';

import { ProjectService, LookupsService } from '../../../api';

export interface ResourceOutput {
  userId: number;
  fullName: string;
  email: string | null;
  roleId: number;
  roleName: string | null;
  projectId: number;
  projectName: string;
}

@Component({
  selector: 'app-resource-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, Tag, MultiSelect, ToastModule, TooltipModule],
  templateUrl: './resource-list.component.html',
  styleUrl: './resource-list.component.scss'
})
export class ResourceListComponent implements OnInit, OnDestroy {
  /** Fixed project scope. When set, only that project's resources are listed (project filter hidden). */
  @Input() projectId?: number;
  /** Hide the Project column when the list is already scoped to one project. */
  @Input() showProjectColumn = true;
  /** Show the search/filter console. */
  @Input() enableSearch = true;

  @ViewChild(Table) private dataTable?: Table;

  private projectService = inject(ProjectService);
  private lookupsService = inject(LookupsService);
  private messageService = inject(MessageService);

  resources = signal<ResourceOutput[]>([]);
  loading = signal(false);

  page = 1;
  pageSize = 10;
  totalRecords = 0;

  // ── Search / filter state ──
  searchTerm = '';
  filterProjectIds: number[] = [];
  filterRoleIds: number[] = [];

  projectOptions = signal<{ label: string; value: number }[]>([]);
  roleOptions = signal<{ label: string; value: number }[]>([]);

  private searchInput$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    if (this.enableSearch) {
      this.loadFilterOptions();
      this.searchInput$
        .pipe(debounceTime(350), distinctUntilChanged(), takeUntil(this.destroy$))
        .subscribe(term => {
          this.searchTerm = term;
          this.reload();
        });
    }
    // Initial fetch is triggered by the lazy table's first onLazyLoad.
  }

  /** Reset to page 1 (paginator + data) after a search/filter change. */
  private reload(): void {
    if (this.dataTable) this.dataTable.first = 0;
    this.load(1);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ── Data ──
  load(page: number, pageSize: number = this.pageSize): void {
    this.loading.set(true);
    const projectIds = this.projectId
      ? [this.projectId]
      : this.filterProjectIds.length ? this.filterProjectIds : undefined;
    const term = this.searchTerm.trim() || undefined;
    const roleIds = this.filterRoleIds.length ? this.filterRoleIds : undefined;

    this.projectService
      .searchResources(term, projectIds, roleIds, page, pageSize)
      .subscribe({
        next: (res: any) => {
          this.resources.set(res?.items ?? []);
          this.totalRecords = res?.totalCount ?? 0;
          this.page = res?.page ?? page;
          this.pageSize = res?.pageSize ?? pageSize;
          this.loading.set(false);
        },
        error: (err) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: err.error?.message || 'Failed to load resources'
          });
          this.loading.set(false);
        }
      });
  }

  onPageChange(event: TableLazyLoadEvent): void {
    const first = event.first ?? 0;
    const rows = event.rows ?? this.pageSize;
    this.load(Math.floor(first / rows) + 1, rows);
  }

  // ── Search handlers ──
  onSearchInput(value: string): void {
    this.searchInput$.next(value);
  }

  onProjectFilterChange(): void {
    this.reload();
  }

  onRoleFilterChange(): void {
    this.reload();
  }

  clearSearchTerm(): void {
    this.searchTerm = '';
    this.reload();
  }

  clearProjectFilter(): void {
    this.filterProjectIds = [];
    this.reload();
  }

  /** Remove a single project from the active selection. */
  removeProject(pid: number): void {
    this.filterProjectIds = this.filterProjectIds.filter(id => id !== pid);
    this.reload();
  }

  /** Remove a single role from the active selection. */
  removeRole(roleId: number): void {
    this.filterRoleIds = this.filterRoleIds.filter(id => id !== roleId);
    this.reload();
  }

  clearRoleFilter(): void {
    this.filterRoleIds = [];
    this.reload();
  }

  clearAll(): void {
    this.searchTerm = '';
    this.filterProjectIds = [];
    this.filterRoleIds = [];
    this.reload();
  }

  get hasActiveFilters(): boolean {
    return !!this.searchTerm.trim() || this.filterProjectIds.length > 0 || this.filterRoleIds.length > 0;
  }

  get showProjectFilter(): boolean {
    // Hide when the list is already scoped to a single project.
    return this.projectId == null;
  }

  projectLabel(id: number | null): string {
    return this.projectOptions().find(o => o.value === id)?.label ?? '';
  }

  roleLabel(id: number | null): string {
    return this.roleOptions().find(o => o.value === id)?.label ?? '';
  }

  // ── Filter option lookups ──
  private loadFilterOptions(): void {
    if (this.showProjectFilter) {
      this.projectService.apiProjectGetAllProjectsGet(1, 1000).subscribe({
        next: (res: any) => {
          const items = res?.items ?? res ?? [];
          this.projectOptions.set(
            items.filter((p: any) => p.id != null).map((p: any) => ({ label: p.name, value: p.id }))
          );
        },
        error: () => {}
      });
    }

    this.lookupsService.apiLookupsRolesGet().subscribe({
      next: (roles) => {
        this.roleOptions.set(
          (roles || []).filter(r => r.id != null).map(r => ({ label: r.name ?? '', value: r.id! }))
        );
      },
      error: () => {}
    });
  }
}
