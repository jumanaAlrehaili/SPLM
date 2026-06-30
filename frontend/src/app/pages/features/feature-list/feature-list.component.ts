import { Component, Input, OnInit, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Table, TableModule, TableLazyLoadEvent } from 'primeng/table';
import { Button } from 'primeng/button';
import { Tag } from 'primeng/tag';
import { IconField } from 'primeng/iconfield';
import { InputIcon } from 'primeng/inputicon';
import { InputText } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { Dialog } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { FeatureFormComponent } from '../feature-form/feature-form.component';
import { FeaturesService, LookupsService } from '../../../api';
import { RoleHelper } from '../../../services/role.helper';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-feature-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, Button, Tag, IconField, InputIcon, InputText, Select, Dialog, FeatureFormComponent, ToastModule, TooltipModule],
  templateUrl: './feature-list.html',
  styleUrl: './feature-list.scss',
})
export class FeatureListComponent implements OnInit {
  @Input() projectId!: number; //from parent component (project details) to know which project's features to fetch.
  @ViewChild('featuresTable') featuresTable!: Table;

  features = signal<any[]>([]);
  loading = signal(false);
  displayFormDialog = signal(false);
  selectedFeature = signal<any | null>(null);

  // ── Pagination ──
  currentPage = 1;
  pageSize = 5;
  totalRecords = 0;
  isSearchMode = false;

  // ── Search ──
  showSearch = false;
  searchName = '';
  searchPriority: number | null = null;
  searchStatusId: number | null = null;

  priorityOptions: { label: string; value: number }[] = [];
  priorityLookupMap = signal<Map<number, string>>(new Map());

  statusOptions: { label: string; value: number }[] = [];

  constructor(
    private featuresService: FeaturesService,
    private lookupsService: LookupsService,
    private roleHelper: RoleHelper,
    private router: Router,
    private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadStatusOptions();
    if (this.projectId) {
      this.fetchFeatures(this.currentPage, this.pageSize);
    }
  }

  private loadStatusOptions(): void {
    this.lookupsService.apiLookupsStatusesGet().subscribe({
      next: (statuses) => {
        this.statusOptions = (statuses || [])
          .filter(s => s.id != null)
          .map(s => ({ label: s.statusName ?? '', value: s.id! }));
      },
      error: () => {}
    });

    this.lookupsService.apiLookupsFeaturePrioritiesGet().subscribe({
      next: (priorities) => {
        const map = new Map<number, string>();
        (priorities || []).forEach(p => { if (p.id != null) map.set(p.id, p.name ?? ''); });
        this.priorityLookupMap.set(map);
        this.priorityOptions = (priorities || [])
          .filter(p => p.id != null)
          .map(p => ({ label: p.name ?? '', value: p.id! }));
      },
      error: () => {}
    });
  }

  isBusinessAnalyst(): boolean {
    const role = this.roleHelper.getUserRole(); // Pull the decoded role name from token
    return role === 'Business Analyst';
  }

  fetchFeatures(page: number = 1, pageSize: number = this.pageSize): void {
    this.loading.set(true);
    this.featuresService.apiProjectsProjectIdFeaturesGetAllFeaturesGet(this.projectId, page, pageSize)
      .subscribe({
        next: (response) => {
          this.features.set(response?.items ?? response ?? []);
          this.totalRecords = response?.totalCount ?? response?.length ?? 0;
          this.currentPage = page;
          this.isSearchMode = false;
          this.loading.set(false);
        },
        error: (err) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to fetch features'
          });
          this.loading.set(false);
        }
      });
  }

  onPageChange(event: TableLazyLoadEvent): void {
    const first = event.first ?? 0;
    const rows = event.rows ?? this.pageSize;
    const page = Math.floor(first / rows) + 1;
    this.pageSize = rows;

    if (this.isSearchMode) {
      this.searchWithPagination(page, rows);
    } else {
      this.fetchFeatures(page, rows);
    }
  }

  //navigate to feature details page
  viewFeature(featureId: number): void {
    this.router.navigate(['/dashboard/projects', this.projectId, 'features', featureId]);
  }

  editFeature(feature: any): void {
    this.selectedFeature.set(feature);
    this.displayFormDialog.set(true);
  }

  deleteFeature(featureId: number): void {
    if (confirm('Are you sure you want to delete this feature record permanently?')) {
      this.featuresService.apiProjectsProjectIdFeaturesDeleteFeatureFeatureIdDelete(this.projectId, featureId)
        .subscribe({
          next: () => {
            this.features.update(items => items.filter(f => f.id !== featureId)); //to remove the deleted feature from the list without refetching all features again from db, 
            this.messageService.add({                                             //and to immediately reflect the change in the UI.
              severity: 'success',
              summary: 'Deleted',
              detail: 'Feature deleted successfully'
            }); 
          },
          error: (err) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Delete Failed',
              detail: 'Unable to delete feature'
            });
          }
        });
    }
  }

  openAddFeatureDialog(): void {
    this.selectedFeature.set(null); //null when adding new.
    this.displayFormDialog.set(true);
  }

  closeFormDialog(): void {
    this.displayFormDialog.set(false);
    this.selectedFeature.set(null);
  }

  toggleSearch(): void {
    this.showSearch = !this.showSearch;
    if (!this.showSearch) this.clearSearch();
  }

  search(): void {
    this.currentPage = 1;
    this.searchWithPagination(1, this.pageSize);
  }

  private searchWithPagination(page: number, pageSize: number): void {
    const name = this.searchName.trim() || undefined;
    const priority = this.searchPriority ?? undefined;
    const statusId = this.searchStatusId ?? undefined;

    this.loading.set(true);
    this.featuresService.apiProjectsProjectIdFeaturesSearchFeaturesGet(this.projectId, name, priority, statusId, page, pageSize)
      .subscribe({
        next: (response) => {
          this.features.set(response?.items ?? response ?? []);
          this.totalRecords = response?.totalCount ?? response?.length ?? 0;
          this.currentPage = page;
          this.isSearchMode = true;
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Feature search failed', err);
          this.loading.set(false);
        }
      });
  }

  clearSearch(): void {
    this.searchName = '';
    this.searchPriority = null;
    this.searchStatusId = null;
    this.isSearchMode = false;
    this.showSearch = false;
    this.currentPage = 1;
    this.fetchFeatures(1, this.pageSize);
  }

  getPriorityLabel(priority: number): string {
    return this.priorityLookupMap().get(priority) ?? '';
  }

  getPrioritySeverity(priority: number): 'success' | 'info' | 'warn' | 'danger' | undefined {
    const name = this.getPriorityLabel(priority).toLowerCase();
    if (name === 'critical') return 'danger';
    if (name === 'high')     return 'warn';
    if (name === 'medium')   return 'info';
    if (name === 'low')      return 'success';
    return undefined;
  }
  //colors for priority (table)
  getStatusSeverity(status: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' | undefined {
    switch (status) {
      case 'New': return 'secondary';
      case 'In Progress': return 'info';
      case 'Pending Review': return 'warn';
      case 'Completed': return 'success';
      case 'Rejected': return 'danger';
      default: return undefined;
    }
  }
}