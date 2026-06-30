import { Component, OnInit, signal, inject, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin, map, of, Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { ActivatedRoute } from '@angular/router';
import {
  ProjectService,
  ReleasePlansService,
  ReleaseStagesService,
  LookupsService
} from '../../../api';
import { RoleHelper } from '../../../services/role.helper';
import { MessageService } from 'primeng/api';
import { UpdateReleaseStageInput } from '../../../api/model/updateReleaseStageInput';
import { CreateReleaseStageInput } from '../../../api/model/createReleaseStageInput';

import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { Tag } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-release-stages-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Button,
    Dialog,
    InputText,
    Select,
    Tag,
    ToastModule,
    TooltipModule
  ],
  templateUrl: './release-stages-page.component.html',
  styleUrl: './release-stages-page.component.scss'
})
export class ReleaseStagePage implements OnInit, OnDestroy {
  private querySub?: Subscription;
  private projectService    = inject(ProjectService);
  private releasePlansService = inject(ReleasePlansService);
  private releaseStagesService = inject(ReleaseStagesService);
  private lookupsService    = inject(LookupsService);
  private roleHelper        = inject(RoleHelper);
  private messageService    = inject(MessageService);
  private route             = inject(ActivatedRoute);

  // ── Selectors ──────────────────────────────────────────────────────────────
  projects         = signal<any[]>([]);
  projectOptions:  { label: string; value: number }[] = [];
  projectsLead         = signal<any[]>([]);

  releasePlans     = signal<any[]>([]);
  planOptions:     { label: string; value: number }[] = [];

  selectedProjectId = signal<number | null>(null);
  selectedPlanId    = signal<number | null>(null);

  // ── Data ───────────────────────────────────────────────────────────────────
  releases             = signal<any[]>([]);
  /** releaseId → stages[] */
  releaseStagesMap     = signal<Map<number, any[]>>(new Map());
  selectedReleaseIndex = signal(0);

  get currentRelease(): any {
    return this.releases()[this.selectedReleaseIndex()] ?? null;
  }

  selectRelease(index: number): void {
    this.selectedReleaseIndex.set(index);
  }

  prevRelease(): void {
    const i = this.selectedReleaseIndex();
    if (i > 0) this.selectedReleaseIndex.set(i - 1);
  }

  nextRelease(): void {
    const i = this.selectedReleaseIndex();
    if (i < this.releases().length - 1) this.selectedReleaseIndex.set(i + 1);
  }

  // ── Loading flags ──────────────────────────────────────────────────────────
  loadingProjects  = signal(false);
  loadingPlans     = signal(false);
  loadingStages    = signal(false);
  loadingProjectLeads = signal(false);

  // ── Lookup maps ────────────────────────────────────────────────────────────
  stageLookupMap   = signal<Map<number, string>>(new Map());
  statusLookupMap  = signal<Map<number, string>>(new Map());
  /** Raw catalog of stages (id, stageName, sequence) for the Add Stage dropdown. */
  catalogStages    = signal<any[]>([]);

  // ── Add stage ──────────────────────────────────────────────────────────────
  addStageId: number | null = null;
  addStageWorkingDays: number | null = null;
  stageCreating = signal(false);

  // ── Pending query params (set before projects load) ───────────────────────
  private pendingProjectId: number | null = null;
  private pendingPlanId:    number | null = null;

  // ── Stage edit dialog ──────────────────────────────────────────────────────
  displayEditStageDialog   = signal(false);
  selectedStageForEdit     = signal<any | null>(null);
  selectedReleaseForEdit   = signal<any | null>(null);
  stageEditName            = '';
  stageEditWorkingDays: number | null = null;
  stageSaving              = signal(false);

  // ── Stage move ─────────────────────────────────────────────────────────────
  stageMoving = signal<number | null>(null);

  ngOnInit(): void {
    this.loadLookups();
    this.querySub = this.route.queryParams.subscribe(params => {
      this.pendingProjectId = params['projectId'] ? +params['projectId'] : null;
      this.pendingPlanId    = params['planId']    ? +params['planId']    : null;
      this.loadProjects();
    });
  }

  ngOnDestroy(): void {
    this.querySub?.unsubscribe();
  }

  isPM(): boolean { return this.roleHelper.isPM(); }

  /**
   * A stage can be managed by the Project Manager, or by the user assigned
   * as the lead of that specific stage (matched on stageId → userId).
   */
  canManageStage(stage: any): boolean {
    if (this.roleHelper.isPM()) return true;
    const userId = this.roleHelper.getUserId();
    if (userId == null || !stage) return false;
    return this.projectsLead().some(
      (l: any) => l.stageId === stage.stageId && Number(l.userId) === userId
    );
  }

  // ── Lookups ────────────────────────────────────────────────────────────────
  private loadLookups(): void {
    this.lookupsService.apiLookupsStagesGet().subscribe({
      next: (stages) => {
        const list = (stages || []).filter(s => s.id != null);
        const map = new Map<number, string>();
        list.forEach(s => map.set(s.id!, s.stageName ?? `Stage ${s.sequence}`));
        this.stageLookupMap.set(map);
        // Keep workflow order by sequence for the Add Stage dropdown.
        this.catalogStages.set(
          [...list].sort((a, b) => (a.sequence ?? 0) - (b.sequence ?? 0))
        );
      },
      error: () => {}
    });

    this.lookupsService.apiLookupsReleaseStageStatusesGet().subscribe({
      next: (statuses) => {
        const map = new Map<number, string>();
        (statuses || []).forEach(s => { if (s.id != null) map.set(s.id, s.statusName ?? ''); });
        this.statusLookupMap.set(map);
      },
      error: () => {}
    });
  }

  // ── Projects ───────────────────────────────────────────────────────────────
  loadProjects(): void {
    this.loadingProjects.set(true);
    this.projectService.apiProjectGetMyProjectsGet().subscribe({
      next: (response) => {
        // Extract items from paginated response (or use raw array as fallback)
        const data = response?.items ?? response ?? [];
        // Pending=2, Member=3, Owner=4 — exclude None (1) only
        const members = (data || []).filter((p: any) => p.membershipStatusId >= 2);
        this.projects.set(members);
        this.projectOptions = members.map((p: any) => ({ label: p.name, value: p.id }));
        if (members.length > 0) {
          const target = this.pendingProjectId && members.some((p: any) => p.id === this.pendingProjectId)
            ? this.pendingProjectId
            : members[0].id;
          this.pendingProjectId = null;
          this.onProjectChange(target);
        }
        this.loadingProjects.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load projects' });
        this.loadingProjects.set(false);
      }
    });
  }

loadProjectLeads(projectId: number): void {
  this.loadingProjectLeads.set(true); // loading = true

  this.projectService.getProjectLeads(projectId).subscribe({
    next: (leads) => {
      // store the result (example)
      this.projectsLead.set(leads);

      this.loadingProjectLeads.set(false); // loading = false
    },
    error: (err) => {
      console.error('Error loading project leads', err);

      this.loadingProjectLeads.set(false); // stop loading on error
    }
  });
}

  onProjectChange(projectId: number): void {
    this.selectedProjectId.set(projectId);
    this.selectedPlanId.set(null);
    this.releasePlans.set([]);
    this.planOptions = [];
    this.releases.set([]);
    this.releaseStagesMap.set(new Map());
    this.loadPlans(projectId);
    this.loadProjectLeads(projectId);
  }

  // ── Release Plans ──────────────────────────────────────────────────────────
  private loadPlans(projectId: number): void {
    this.loadingPlans.set(true);
    this.releasePlansService.getReleasePlans(projectId).subscribe({
      next: (plans) => {
        this.releasePlans.set(plans || []);
        this.planOptions = (plans || []).map((p: any) => ({ label: p.name, value: p.id }));
        if (plans && plans.length > 0) {
          const target = this.pendingPlanId && plans.some((p: any) => p.id === this.pendingPlanId)
            ? this.pendingPlanId
            : plans[0].id;
          this.pendingPlanId = null;
          this.onPlanChange(target);
        }
        this.loadingPlans.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load release plans' });
        this.loadingPlans.set(false);
      }
    });
  }

  onPlanChange(planId: number): void {
    this.selectedPlanId.set(planId);
    this.releases.set([]);
    this.releaseStagesMap.set(new Map());
    this.loadReleasesAndStages(planId);
  }

  // ── Releases + Stages ──────────────────────────────────────────────────────
  private loadReleasesAndStages(planId: number): void {
    const projectId = this.selectedProjectId();
    if (!projectId) return;

    this.loadingStages.set(true);

    this.releasePlansService.getAllReleases(projectId, planId)
      .subscribe({
        next: (releases) => {
          this.releases.set(releases || []);

          if (!releases || releases.length === 0) {
            this.loadingStages.set(false);
            return;
          }

          type StageResult = { releaseId: number; stages: any[] };

          const stageRequests: Observable<StageResult>[] = releases.map((r: any) =>
            this.releaseStagesService
              .getAllReleaseStages(
                projectId, planId, r.id
              )
              .pipe(
                map((stages): StageResult => ({ releaseId: r.id, stages: stages || [] })),
                catchError((): Observable<StageResult> => of({ releaseId: r.id, stages: [] }))
              )
          );

          forkJoin(stageRequests).subscribe({
            next: (results: StageResult[]) => {
              const map = new Map<number, any[]>();
              results.forEach(r => map.set(r.releaseId, r.stages));
              this.releaseStagesMap.set(map);
              this.selectedReleaseIndex.set(0);
              this.loadingStages.set(false);
            },
            error: () => this.loadingStages.set(false)
          });
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load releases' });
          this.loadingStages.set(false);
        }
      });
  }

  refreshRelease(release: any): void {
    const projectId = this.selectedProjectId();
    const planId    = this.selectedPlanId();
    if (!projectId || !planId) return;

    this.releaseStagesService
      .getAllReleaseStages(
        projectId, planId, release.id
      )
      .subscribe({
        next: (stages) => {
          this.releaseStagesMap.update(m => {
            const next = new Map(m);
            next.set(release.id, stages || []);
            return next;
          });
        },
        error: () => {}
      });
  }

  getStagesForRelease(releaseId: number): any[] {
    return this.releaseStagesMap().get(releaseId) ?? [];
  }

  // ── Add stage (PM only) ──────────────────────────────────────────────────────

  /** Catalog dropdown options; already-added stages are disabled. */
  getAddStageOptions(release: any): any[] {
    const usedStageIds = new Set(this.getStagesForRelease(release?.id).map((s: any) => s.stageId));
    return this.catalogStages().map(s => ({
      label: s.stageName ?? `Stage ${s.sequence}`,
      value: s.id,
      disabled: usedStageIds.has(s.id)
    }));
  }

  resetAddStageForm(): void {
    this.addStageId = null;
    this.addStageWorkingDays = null;
  }

  createStage(release: any): void {
    const projectId = this.selectedProjectId();
    const planId    = this.selectedPlanId();
    if (!projectId || !planId || !release) return;

    if (this.addStageId == null) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Select a stage to add' });
      return;
    }
    if (this.addStageWorkingDays == null || this.addStageWorkingDays < 1) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Working days must be at least 1' });
      return;
    }

    this.stageCreating.set(true);
    const input: CreateReleaseStageInput = {
      stageId: this.addStageId,
      workingDays: this.addStageWorkingDays
    };

    this.releaseStagesService
      .createReleaseStage(projectId, planId, release.id, input)
      .subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Stage Added', detail: 'Stage added to the release' });
          this.stageCreating.set(false);
          this.resetAddStageForm();
          this.refreshRelease(release);
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to add stage' });
          this.stageCreating.set(false);
        }
      });
  }

  // ── Stage move ─────────────────────────────────────────────────────────────
  moveStage(stage: any, release: any): void {
    const projectId = this.selectedProjectId();
    const planId    = this.selectedPlanId();
    if (!projectId || !planId) return;

    this.stageMoving.set(stage.id);
    this.releaseStagesService
      .moveReleaseStage(
        projectId, planId, release.id, stage.id
      )
      .subscribe({
        next: () => {
          const nextLabel = this.getStageStatusString(stage) === 'Not Started' ? 'In Progress' : 'Completed';
          this.messageService.add({ severity: 'success', summary: 'Stage Updated', detail: `Stage moved to "${nextLabel}"` });
          this.stageMoving.set(null);
          this.refreshRelease(release);
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to advance stage' });
          this.stageMoving.set(null);
        }
      });
  }

  // ── Stage edit ─────────────────────────────────────────────────────────────
  openEditStageDialog(stage: any, release: any): void {
    this.selectedStageForEdit.set(stage);
    this.selectedReleaseForEdit.set(release);
    this.stageEditName        = stage.stageName?.trim() ?? '';
    this.stageEditWorkingDays = stage.workingDays ?? null;
    this.displayEditStageDialog.set(true);
  }

  saveStageEdit(): void {
    const projectId = this.selectedProjectId();
    const planId    = this.selectedPlanId();
    const stage     = this.selectedStageForEdit();
    const release   = this.selectedReleaseForEdit();
    if (!projectId || !planId || !stage || !release) return;

    if (this.stageEditWorkingDays == null || this.stageEditWorkingDays < 0) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Working days is required' });
      return;
    }

    this.stageSaving.set(true);
    const input: UpdateReleaseStageInput = {
      stageName:   this.stageEditName?.trim() || null,
      workingDays: this.stageEditWorkingDays
    };

    this.releaseStagesService
      .updateReleaseStage(
        projectId, planId, release.id, stage.id, input
      )
      .subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Saved', detail: 'Stage updated successfully' });
          this.displayEditStageDialog.set(false);
          this.stageSaving.set(false);
          this.refreshRelease(release);
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to update stage' });
          this.stageSaving.set(false);
        }
      });
  }

  // ── Helpers ────────────────────────────────────────────────────────────────
  getStageStatusString(stage: any): string {
    if (!stage) return '';
    if (stage.statusId != null) return this.statusLookupMap().get(stage.statusId) ?? '';
    if (typeof stage.status === 'string') return stage.status;
    return stage.status?.statusName ?? '';
  }

  getStageName(stage: any): string {
    if (stage.stageName?.trim()) return stage.stageName.trim();
    if (stage.stageId != null) return this.stageLookupMap().get(stage.stageId) ?? `Stage ${stage.sequence}`;
    return `Stage ${stage.sequence ?? ''}`;
  }

  getStageStatusSeverity(stage: any): 'success' | 'info' | 'secondary' {
    const s = this.getStageStatusString(stage);
    if (s === 'Completed')  return 'success';
    if (s === 'In Progress') return 'info';
    return 'secondary';
  }

  canMoveStage(stage: any, index: number, stages: any[]): boolean {
    if (this.getStageStatusString(stage) === 'Completed') return false;
    if (index === 0) return true;
    return this.getStageStatusString(stages[index - 1]) === 'Completed';
  }

  getMoveButtonLabel(stage: any): string {
    const s = this.getStageStatusString(stage);
    if (s === 'Not Started') return 'Start';
    if (s === 'In Progress') return 'Complete';
    return '';
  }

  getCompletedCount(stages: any[]): number {
    return stages.filter(s => this.getStageStatusString(s) === 'Completed').length;
  }

  getReleaseProgressPct(stages: any[]): number {
    if (!stages.length) return 0;
    return Math.round((this.getCompletedCount(stages) / stages.length) * 100);
  }

  getReleaseOverallStatus(stages: any[]): string {
    if (!stages.length) return 'No stages';
    const completed = this.getCompletedCount(stages);
    if (completed === stages.length) return 'Completed';
    if (stages.some(s => this.getStageStatusString(s) === 'In Progress')) return 'In Progress';
    return 'Not Started';
  }

  getReleaseOverallSeverity(stages: any[]): 'success' | 'info' | 'secondary' {
    const s = this.getReleaseOverallStatus(stages);
    if (s === 'Completed')  return 'success';
    if (s === 'In Progress') return 'info';
    return 'secondary';
  }

  formatDate(dateStr: string | null | undefined): string {
    if (!dateStr) return '—';
    try {
      return new Date(dateStr).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
    } catch { return dateStr; }
  }
}
