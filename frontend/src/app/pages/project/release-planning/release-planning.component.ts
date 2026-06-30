import { Component, Input, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReleasePlansService, FeaturesService, ReleaseStagesService, LookupsService } from '../../../api';
import { Router } from '@angular/router';
import { RoleHelper } from '../../../services/role.helper';
import { MessageService } from 'primeng/api';
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { Tag } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { UpdateReleaseStageInput } from '../../../api/model/updateReleaseStageInput';

@Component({
  selector: 'app-release-planning',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Button,
    Dialog,
    InputText,
    TableModule,
    ToastModule,
    Tag,
    TooltipModule
  ],
  templateUrl: './release-planning.component.html',
  styleUrl: './release-planning.component.scss'
})
export class ReleasePlanningComponent implements OnInit {
  private releasePlansService = inject(ReleasePlansService);
  private featuresService = inject(FeaturesService);
  private releaseStagesService = inject(ReleaseStagesService);
  private lookupsService = inject(LookupsService);
  private roleHelper = inject(RoleHelper);
  private messageService = inject(MessageService);
  private router = inject(Router);

  private _projectId!: number;

  @Input({ required: true })
  set projectId(val: number) {
    this._projectId = val;
    if (val) {
      this.selectedPlan.set(null);
      this.releasePlans.set([]);
      this.features.set([]);
      this.loadAllData();
    }
  }

  get projectId(): number {
    return this._projectId;
  }

  // --- Signals & State ---
  releasePlans = signal<any[]>([]);
  selectedPlan = signal<any | null>(null);
  features = signal<any[]>([]);
  loading = signal<boolean>(false);
  submitting = signal<boolean>(false);

  // --- Dialog Controls ---
  displayPlanDialog = signal<boolean>(false);
  displayReleaseDialog = signal<boolean>(false);
  displayFeatureAssignmentDialog = signal<boolean>(false);

  // --- Selection Trackers for CRUD ---
  selectedPlanForEdit = signal<any | null>(null);
  selectedReleaseForEdit = signal<any | null>(null);
  selectedReleaseForAssignment = signal<any | null>(null);

  // --- Form Models ---
  planName = '';
  planDescription = '';
  releaseName = '';
  releaseDescription = '';

  // ===== Lookup Maps =====
  stageLookupMap    = signal<Map<number, string>>(new Map());
  statusLookupMap   = signal<Map<number, string>>(new Map());
  priorityLookupMap = signal<Map<number, string>>(new Map());

  viewStages(release: any): void {
    const plan = this.selectedPlan();
    this.router.navigate(['/dashboard/release-stages'], {
      queryParams: { projectId: this.projectId, planId: plan?.id }
    });
  }

  // ===== Stage Management State =====
  selectedReleaseForStages = signal<any | null>(null);
  stages = signal<any[]>([]);
  stagesLoading = signal<boolean>(false);
  displayStagesDialog = signal<boolean>(false);

  displayEditStageDialog = signal<boolean>(false);
  selectedStageForEdit = signal<any | null>(null);
  stageEditName = '';
  stageEditWorkingDays: number | null = null;
  stageSaving = signal<boolean>(false);
  stageMoving = signal<number | null>(null);

  ngOnInit(): void {
    this.loadLookups();
  }

  private loadLookups(): void {
    this.lookupsService.apiLookupsStagesGet().subscribe({
      next: (stages) => {
        const map = new Map<number, string>();
        (stages || []).forEach(s => {
          if (s.id != null) map.set(s.id, s.stageName ?? `Stage ${s.sequence}`);
        });
        this.stageLookupMap.set(map);
      },
      error: () => {}
    });

    this.lookupsService.apiLookupsReleaseStageStatusesGet().subscribe({
      next: (statuses) => {
        const map = new Map<number, string>();
        (statuses || []).forEach(s => {
          if (s.id != null) map.set(s.id, s.statusName ?? '');
        });
        this.statusLookupMap.set(map);
      },
      error: () => {}
    });

    this.lookupsService.apiLookupsFeaturePrioritiesGet().subscribe({
      next: (priorities) => {
        const map = new Map<number, string>();
        (priorities || []).forEach(p => { if (p.id != null) map.set(p.id, p.name ?? ''); });
        this.priorityLookupMap.set(map);
      },
      error: () => {}
    });
  }

  isPM(): boolean { return this.roleHelper.isPM(); }
  canManageStage(): boolean { return this.roleHelper.canManageStage(); }

  loadAllData(): void {
    this.loading.set(true);
    this.fetchReleasePlans(() => {
      this.fetchFeatures();
    });
  }

  fetchReleasePlans(callback?: () => void): void {
    this.releasePlansService.getReleasePlans(this.projectId)
      .subscribe({
        next: (plans) => {
          this.releasePlans.set(plans || []);
          if (plans && plans.length > 0) {
            const currentSelected = this.selectedPlan();
            let targetPlan = plans[0];
            if (currentSelected) {
              const updatedPlan = plans.find((p: any) => p.id === currentSelected.id);
              targetPlan = updatedPlan || plans[0];
            }
            this.selectedPlan.set(targetPlan);
            this.fetchReleasesForPlan(targetPlan.id, callback);
          } else {
            this.selectedPlan.set(null);
            if (callback) callback();
            else this.loading.set(false);
          }
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to fetch release plans' });
          if (callback) callback();
          else this.loading.set(false);
        }
      });
  }

  fetchReleasesForPlan(planId: number, callback?: () => void): void {
    this.releasePlansService.getAllReleases(this.projectId, planId)
      .subscribe({
        next: (releases) => {
          const plan = this.selectedPlan();
          if (plan && plan.id === planId) {
            this.selectedPlan.set({ ...plan, releases: releases || [] });
          }
          if (callback) callback();
          else this.loading.set(false);
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to fetch releases for the active plan' });
          if (callback) callback();
          else this.loading.set(false);
        }
      });
  }

  fetchFeatures(): void {
    this.featuresService.apiProjectsProjectIdFeaturesGetAllFeaturesGet(this.projectId)
      .subscribe({
        next: (response) => {
          // Extract items from paginated response (or use raw array as fallback)
          const data = response?.items ?? response ?? [];
          this.features.set(data || []);
          this.loading.set(false);
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to fetch features list' });
          this.loading.set(false);
        }
      });
  }

  selectPlan(plan: any): void {
    this.selectedPlan.set(plan);
    if (plan) {
      this.loading.set(true);
      this.fetchReleasesForPlan(plan.id);
    }
  }

  // --- Release Plan CRUD ---
  openCreatePlanDialog(): void {
    this.selectedPlanForEdit.set(null);
    this.planName = '';
    this.planDescription = '';
    this.displayPlanDialog.set(true);
  }

  openEditPlanDialog(plan: any, event: Event): void {
    event.stopPropagation();
    this.selectedPlanForEdit.set(plan);
    this.planName = plan.name;
    this.planDescription = plan.description || '';
    this.displayPlanDialog.set(true);
  }

  savePlan(): void {
    if (!this.planName.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Name is required' });
      return;
    }
    this.submitting.set(true);
    const planInput = { name: this.planName.trim(), description: this.planDescription.trim() || null };
    const editPlan = this.selectedPlanForEdit();
    if (editPlan) {
      this.releasePlansService.updateReleasePlan(this.projectId, editPlan.id, planInput)
        .subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Release plan updated' });
            this.displayPlanDialog.set(false);
            this.submitting.set(false);
            this.fetchReleasePlans();
          },
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to update release plan' });
            this.submitting.set(false);
          }
        });
    } else {
      this.releasePlansService.createReleasePlan(this.projectId, planInput)
        .subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Release plan created' });
            this.displayPlanDialog.set(false);
            this.submitting.set(false);
            this.fetchReleasePlans();
          },
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to create release plan' });
            this.submitting.set(false);
          }
        });
    }
  }

  deletePlan(plan: any, event: Event): void {
    event.stopPropagation();
    if (!confirm(`Are you sure you want to delete "${plan.name}" permanently? All releases under this plan will also be affected.`)) return;
    this.loading.set(true);
    this.releasePlansService.deleteReleasePlan(this.projectId, plan.id)
      .subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Release plan deleted' });
          if (this.selectedPlan()?.id === plan.id) this.selectedPlan.set(null);
          this.fetchReleasePlans();
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to delete release plan' });
          this.loading.set(false);
        }
      });
  }

  // --- Release CRUD ---
  openCreateReleaseDialog(): void {
    if (!this.selectedPlan()) {
      this.messageService.add({ severity: 'warn', summary: 'Warning', detail: 'Please select a release plan first' });
      return;
    }
    this.selectedReleaseForEdit.set(null);
    this.releaseName = '';
    this.releaseDescription = '';
    this.displayReleaseDialog.set(true);
  }

  openEditReleaseDialog(release: any): void {
    this.selectedReleaseForEdit.set(release);
    this.releaseName = release.name;
    this.releaseDescription = release.description || '';
    this.displayReleaseDialog.set(true);
  }

  saveRelease(): void {
    const activePlan = this.selectedPlan();
    if (!activePlan) return;
    if (!this.releaseName.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Release name is required' });
      return;
    }
    this.submitting.set(true);
    const editRelease = this.selectedReleaseForEdit();
    if (editRelease) {
      const releaseInput = { name: this.releaseName.trim(), description: this.releaseDescription.trim() || null };
      this.releasePlansService.updateRelease(this.projectId, activePlan.id, editRelease.id, releaseInput)
        .subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Release updated successfully' });
            this.displayReleaseDialog.set(false);
            this.submitting.set(false);
            this.fetchReleasePlans();
          },
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to update release' });
            this.submitting.set(false);
          }
        });
    } else {
      const releaseInput = { name: this.releaseName.trim(), description: this.releaseDescription.trim() || null };
      this.releasePlansService.createRelease(this.projectId, activePlan.id, releaseInput)
        .subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Release created successfully' });
            this.displayReleaseDialog.set(false);
            this.submitting.set(false);
            this.fetchReleasePlans();
          },
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to create release' });
            this.submitting.set(false);
          }
        });
    }
  }

  deleteRelease(releaseId: number): void {
    const activePlan = this.selectedPlan();
    if (!activePlan) return;
    if (!confirm('Are you sure you want to delete this release record permanently?')) return;
    this.loading.set(true);
    this.releasePlansService.deleteRelease(this.projectId, activePlan.id, releaseId)
      .subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Release deleted successfully' });
          this.fetchReleasePlans();
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to delete release' });
          this.loading.set(false);
        }
      });
  }

  // --- Feature Assignment ---
  openFeatureAssignmentDialog(release: any): void {
    this.selectedReleaseForAssignment.set(release);
    this.displayFeatureAssignmentDialog.set(true);
  }

  isFeatureInRelease(feature: any, release: any): boolean {
    if (!feature || !release) return false;
    if (feature.releaseId === release.id) return true;
    if (feature.release?.id === release.id) return true;
    if (feature.release === release.name) return true;
    return false;
  }

  getFeaturesAssignedToRelease(release: any): any[] {
    if (!release) return [];
    return this.features().filter(f => this.isFeatureInRelease(f, release));
  }

  onFeatureToggle(feature: any, event: any): void {
    const release = this.selectedReleaseForAssignment();
    if (!release) return;
    const checked = event.target.checked;
    const targetReleaseId = checked ? release.id : null;
    this.featuresService.apiProjectsProjectIdFeaturesAssignFeatureToReleaseFeatureIdPut(
      this.projectId, feature.id, { releaseId: targetReleaseId }
    ).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Assigned',
          detail: checked ? `Feature assigned to ${release.name}` : 'Feature removed from release'
        });
        this.features.update(list =>
          list.map(f => f.id === feature.id ? { ...f, releaseId: targetReleaseId, release: checked ? release.name : null } : f)
        );
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Assignment Failed', detail: err.error?.message || 'Failed to toggle feature assignment' });
        event.target.checked = !checked;
      }
    });
  }

  // ===== Stage Management =====

  openStagesDialog(release: any): void {
    this.selectedReleaseForStages.set(release);
    this.stages.set([]);
    this.displayStagesDialog.set(true);
    this.loadStages(release);
  }

  loadStages(release: any): void {
    const plan = this.selectedPlan();
    if (!plan || !release) return;
    this.stagesLoading.set(true);
    this.releaseStagesService.getAllReleaseStages(
      this.projectId, plan.id, release.id
    ).subscribe({
      next: (data) => {
        this.stages.set(data || []);
        this.stagesLoading.set(false);
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to load stages' });
        this.stagesLoading.set(false);
      }
    });
  }

  moveStage(stage: any): void {
    const plan = this.selectedPlan();
    const release = this.selectedReleaseForStages();
    if (!plan || !release) return;
    this.stageMoving.set(stage.id);
    this.releaseStagesService.moveReleaseStage(
      this.projectId, plan.id, release.id, stage.id
    ).subscribe({
      next: () => {
        const nextLabel = this.getStageStatusString(stage) === 'Not Started' ? 'In Progress' : 'Completed';
        this.messageService.add({ severity: 'success', summary: 'Stage Updated', detail: `Stage moved to "${nextLabel}"` });
        this.stageMoving.set(null);
        this.loadStages(release);
        this.fetchReleasesForPlan(plan.id);
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to advance stage' });
        this.stageMoving.set(null);
      }
    });
  }

  openEditStageDialog(stage: any): void {
    this.selectedStageForEdit.set(stage);
    this.stageEditName = stage.stageName?.trim() ?? '';
    this.stageEditWorkingDays = stage.workingDays ?? null;
    this.displayEditStageDialog.set(true);
  }

  saveStageEdit(): void {
    const plan = this.selectedPlan();
    const release = this.selectedReleaseForStages();
    const stage = this.selectedStageForEdit();
    if (!plan || !release || !stage) return;
    if (this.stageEditWorkingDays == null || this.stageEditWorkingDays < 0) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Working days is required' });
      return;
    }
    this.stageSaving.set(true);
    const input: UpdateReleaseStageInput = {
      stageName: this.stageEditName?.trim() || null,
      workingDays: this.stageEditWorkingDays
    };
    this.releaseStagesService.updateReleaseStage(
      this.projectId, plan.id, release.id, stage.id, input
    ).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Saved', detail: 'Stage updated successfully' });
        this.displayEditStageDialog.set(false);
        this.stageSaving.set(false);
        this.loadStages(release);
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to update stage' });
        this.stageSaving.set(false);
      }
    });
  }

  /** Resolves status string from statusId using the lookup map */
  getStageStatusString(stage: any): string {
    if (!stage) return '';
    if (stage.statusId != null) {
      return this.statusLookupMap().get(stage.statusId) ?? '';
    }
    // Fallback for any other shape
    if (typeof stage.status === 'string') return stage.status;
    if (stage.status?.statusName) return stage.status.statusName;
    return '';
  }

  /** Resolves display name: custom stageName field overrides the system lookup */
  getStageName(stage: any): string {
    if (stage.stageName?.trim()) return stage.stageName.trim();
    if (stage.stageId != null) {
      return this.stageLookupMap().get(stage.stageId) ?? `Stage ${stage.sequence}`;
    }
    return `Stage ${stage.sequence ?? ''}`;
  }

  getMoveButtonLabel(stage: any): string {
    const status = this.getStageStatusString(stage);
    if (status === 'Not Started') return 'Start';
    if (status === 'In Progress') return 'Complete';
    return '';
  }

  canMoveStage(stage: any, index: number): boolean {
    const status = this.getStageStatusString(stage);
    if (status === 'Completed') return false;
    if (index === 0) return true;
    const prev = this.stages()[index - 1];
    return this.getStageStatusString(prev) === 'Completed';
  }

  getStageStatusSeverity(stage: any): 'success' | 'info' | 'secondary' {
    const status = this.getStageStatusString(stage);
    if (status === 'Completed') return 'success';
    if (status === 'In Progress') return 'info';
    return 'secondary';
  }

  getCompletedStageCount(): number {
    return this.stages().filter(s => this.getStageStatusString(s) === 'Completed').length;
  }

  formatDate(dateStr: string | null | undefined): string {
    if (!dateStr) return '—';
    try {
      const d = new Date(dateStr);
      return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
    } catch {
      return dateStr;
    }
  }

  // --- Utility Helpers ---
  getPriorityLabel(priority: number): string {
    return this.priorityLookupMap().get(priority) ?? 'Unknown';
  }

  getPrioritySeverity(priority: number): 'success' | 'info' | 'warn' | 'danger' | undefined {
    const name = this.getPriorityLabel(priority).toLowerCase();
    if (name === 'critical') return 'danger';
    if (name === 'high')     return 'warn';
    if (name === 'medium')   return 'info';
    if (name === 'low')      return 'success';
    return undefined;
  }

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
