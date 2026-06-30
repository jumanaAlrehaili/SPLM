import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Tag } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { FeatureAssignmentDto, FeaturesService, LookupsService } from '../../../api';
import { FeatureStageLogOutput } from '../../../api';
import { TimelineModule } from 'primeng/timeline';
import { TableModule } from 'primeng/table';
import { AvailableStageResourceDto } from '../../../api';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { Select } from 'primeng/select';
import { RoleHelper } from '../../../services/role.helper';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';

@Component({
  selector: 'app-feature-detail',
  standalone: true,
  imports: [CommonModule, Tag, ToastModule, ButtonModule, TimelineModule, TableModule, FormsModule, SelectModule, Select, DialogModule, TextareaModule],
  templateUrl: './feature-detail.html',
  styleUrl: './feature-detail.scss'
})
export class FeatureDetailComponent implements OnInit {
  // Extracted from the URL parameters
  projectId!: number;
  featureId!: number;
  feature = signal<any | null>(null);
  loading = signal(false);
  logs = signal<FeatureStageLogOutput[]>([]);
  availableResources = signal<AvailableStageResourceDto[]>([]);
  showCommentDialog: boolean = false;
  rejectionComment: string = '';
  currentStageIdForReject: number | null = null;
  priorityLookupMap = signal<Map<number, string>>(new Map());
  submittingStage = signal(false);
  allStages: any[] = [];
  qaStageId: number | null = null;
  uatStageId: number | null = null;

  //Estimation properties
  showEstimationDialog = false;
  estimationValue: number | null = null;
  selectedUnitId: number | null = null;
  currentStageIdForEstimation: number | null = null;
  estimationUnits = signal<any[]>([]);

  constructor(
    private route: ActivatedRoute,
    private featuresService: FeaturesService,
    private lookupsService: LookupsService,
    private messageService: MessageService,
    private router: Router,
    private roleHelper: RoleHelper
  ) { }

  get currentUserId(): number {
    return this.roleHelper.getUserId() || 0;
  }
  get isManager(): boolean {
    return this.roleHelper.canManageStage();
  }

  ngOnInit(): void {
    this.projectId = Number(this.route.snapshot.paramMap.get('projectId'));
    this.featureId = Number(this.route.snapshot.paramMap.get('featureId'));

    this.lookupsService.apiLookupsFeaturePrioritiesGet().subscribe({
      next: (priorities) => {
        const map = new Map<number, string>();
        (priorities || []).forEach(p => { if (p.id != null) map.set(p.id, p.name ?? ''); });
        this.priorityLookupMap.set(map);
      },
      error: () => { }
    });

    if (this.projectId && this.featureId) {
      this.loadFeatureDetails();
    }

    //get all stages 
    this.lookupsService.apiLookupsStagesGet().subscribe(data => {
      this.allStages = data;
      this.qaStageId = data.find(s => s.stageName === 'QA Stage')?.id || null;
      this.uatStageId = data.find(s => s.stageName === 'UAT Stage')?.id || null;
    });

    //get estimation units
    this.lookupsService.apiLookupsEstimationUnitsGet().subscribe(units => {
      this.estimationUnits.set(units);
    });
  }

  loadFeatureDetails(): void {
    this.loading.set(true);
    this.featuresService.apiProjectsProjectIdFeaturesGetFeatureByIdFeatureIdGet(this.projectId, this.featureId)
      .subscribe({
        next: (data) => {
          this.feature.set(data);
          this.loadLogs();
          this.loading.set(false);
        },
        error: (err) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to fetch feature details'
          });
          this.loading.set(false);
        }
      });
  }

  async loadResourcesForStage(stageId: number) {
    try {
      const resources = await this.featuresService.getAvailableResources(this.projectId, stageId).toPromise();
      this.availableResources.set(resources || []);
    } catch (error) {
      console.error('Failed to load resources', error);
    }
  }

  async onAssign(userId: number | null, stageId: number) {
    try {
      if (userId === null) {
        // unassign
        await this.featuresService.unassignUserFromFeature(this.projectId, this.featureId, stageId).toPromise();
        this.messageService.add({ severity: 'info', summary: 'Unassigned', detail: 'User removed from stage.' });
      } else {
        // assign
        await this.featuresService.assignUserToStageInFeature(this.projectId, this.featureId, stageId, { assignedUserId: userId }).toPromise();
        this.messageService.add({ severity: 'success', summary: 'Assigned', detail: 'User assigned successfully.' });
      }
      this.loadFeatureDetails();
    } catch (error) {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Operation failed.' });
      console.error('Assignment/Unassignment failed', error);
    }
  }

  onCompleteStage(stageId: number) {
    this.moveToNext(stageId);
  }

  moveToNext(stageId: number): void {
    if (this.submittingStage()) return; // Prevent multiple clicks
    this.submittingStage.set(true);  // lock button 

    this.featuresService.moveToNextStageInFeature(
      this.projectId, this.featureId, stageId
    ).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Stage completed!' });
        this.loadFeatureDetails(); // Refresh UI
        this.loadLogs();
        this.submittingStage.set(false); //unlock button for next stages
      },
      error: (err) => {
        this.submittingStage.set(false);
        const errorMessage = err.error?.message || err.error || 'Cannot complete this stage.';
        this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage });
      }
    });
  }

  canComplete(assignment: FeatureAssignmentDto, index: number, assignments: FeatureAssignmentDto[]): boolean {
    if (assignment.completedAt != null) return false;

    // the first stage is always available to start or the previous stage must be completed first
    const isSequenceValid = (index === 0) || (assignments[index - 1].completedAt != null);
    const isAssignedToMe = assignment.userId === this.currentUserId;

    return isSequenceValid && isAssignedToMe;
  }

  canReject(assignment: FeatureAssignmentDto, index: number, assignments: FeatureAssignmentDto[]): boolean {
    if (assignment.completedAt != null) return false;

    const isRejectableStage = assignment.stageId === this.qaStageId || assignment.stageId === this.uatStageId;
    const isSequenceValid = (index === 0) || (assignments[index - 1].completedAt != null);
    const isAssignedToMe = assignment.userId === this.currentUserId;

    return !!isRejectableStage && isSequenceValid && isAssignedToMe;
  }

  rejectStage(stageId: number): void {
    this.currentStageIdForReject = stageId;
    this.showCommentDialog = true;
  }
  submitRejection(): void {
    if (!this.currentStageIdForReject || !this.rejectionComment.trim()) return;
    const payload = { comment: this.rejectionComment };
    this.featuresService.rejectFeatureStage(
      this.projectId, this.featureId, this.currentStageIdForReject, payload
    ).subscribe({
      next: () => {
        this.showCommentDialog = false; //close the dialog after submit 
        this.messageService.add({ severity: 'warn', summary: 'Rejected', detail: 'Stage rejected successfully' });
        this.loadFeatureDetails(); // Refresh UI
      },
      error: (err) => {
        const errorMessage = err.error?.message || err.error || 'Operation failed.';
        this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage })
      }
    });
  }

  isStageCompleted(index: number): boolean {
    return this.feature().assignments[index]?.completedAt != null;
  }

  isActiveStage(index: number, assignments: any[]): boolean {
    return !this.isStageCompleted(index) && (index === 0 || this.isStageCompleted(index - 1));
  }

  isPathDone(index: number): boolean {
    return index > 0 && this.isStageCompleted(index - 1);
  }

  loadLogs(): void {
    this.featuresService.getFeatureLogs(this.projectId, this.featureId)
      .subscribe({
        next: (data) => this.logs.set(data),
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load logs' });
        }
      });
  }

  onStartStage(stageId: number): void {
    this.featuresService.startStage(this.projectId, this.featureId, stageId).subscribe({
      next: () => {
        this.messageService.add({ severity: 'info', summary: 'Started', detail: 'Stage started successfully' });
        this.loadFeatureDetails();
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Could not start stage' });
      }
    });
  }

  openEstimationDialog(assignment: any) {
    this.currentStageIdForEstimation = assignment.stageId;
    this.estimationValue = assignment.estimation;
    this.selectedUnitId = assignment.estimationUnitId;
    this.showEstimationDialog = true;
  }

 submitEstimation() {
  if (!this.currentStageIdForEstimation || !this.estimationValue || !this.selectedUnitId) return;

  this.featuresService.setEstimation(
    this.projectId,
    this.featureId,
    this.currentStageIdForEstimation,
    { estimation: this.estimationValue, estimationUnitId: this.selectedUnitId }
  ).subscribe({
    next: () => {
      this.showEstimationDialog = false;
      this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Estimation saved!' });
      this.loadFeatureDetails();
    },
    error: (err) => {
      const errorMessage = err.error?.message || err.error || 'Failed to update estimation.';
      
      this.messageService.add({ 
        severity: 'error', 
        summary: 'Cannot Update', 
        detail: errorMessage 
      });
      
      this.showEstimationDialog = false; 
    }
  });
}

  getPriorityLabel(priority: number): string {
    return this.priorityLookupMap().get(priority) ?? '';
  }

  getPrioritySeverity(priority: number): 'success' | 'info' | 'warn' | 'danger' | undefined {
    const name = this.getPriorityLabel(priority).toLowerCase();
    if (name === 'critical') return 'danger';
    if (name === 'high') return 'warn';
    if (name === 'medium') return 'info';
    if (name === 'low') return 'success';
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

  goBack(): void {
    this.router.navigate(['/dashboard/projects', this.projectId]);
  }
}