import { Component, Input, Output, EventEmitter, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { Select } from 'primeng/select';
import { DatePicker } from 'primeng/datepicker';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { FeaturesService, LookupsService } from '../../../api';
import { CreateFeatureInput } from '../../../api';
import { UpdateFeatureInput } from '../../../api';

@Component({
  selector: 'app-feature-form',
  standalone: true,
  imports: [CommonModule, Button, InputText, Textarea, Select, DatePicker, ReactiveFormsModule, ToastModule],
  templateUrl: './feature-form.html',
  styleUrl: './feature-form.scss',
})
export class FeatureFormComponent implements OnInit {
  @Input() projectId!: number;
  @Input() featureData: any | null = null;
  @Output() onClose = new EventEmitter<any>();

  featureForm!: FormGroup;
  isSubmitting = signal(false);

  priorityOptions = signal<{ label: string; value: number }[]>([]);

  constructor(
    private fb: FormBuilder,
    private featuresService: FeaturesService,
    private lookupsService: LookupsService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadPriorityOptions();
    if (this.featureData) {
      this.populateForm(this.featureData);
    }
  }

  private loadPriorityOptions(): void {
    this.lookupsService.apiLookupsFeaturePrioritiesGet().subscribe({
      next: (priorities) => {
        const options = (priorities || [])
          .filter(p => p.id != null)
          .map(p => ({ label: p.name ?? '', value: p.id! }));
        this.priorityOptions.set(options);

        // Set default to Medium if not editing an existing feature
        if (!this.featureData) {
          const medium = options.find(p => p.label.toLowerCase() === 'medium');
          if (medium) this.featureForm.patchValue({ priority: medium.value });
        }
      },
      error: () => {}
    });
  }

  private initForm(): void {
    this.featureForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      priority: [null, Validators.required],
      epicLink: ['']
    });
  }

  private populateForm(data: any): void {
    this.featureForm.patchValue({
      title: data.title,
      description: data.description,
      priority: data.priority,
      epicLink: data.epicLink
    });
  }

  onSubmit(): void {
    if (this.featureForm.invalid) {
      this.featureForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    if (this.featureData) {
      // Prepare UpdateFeatureInput mapping for update.
      const updatePayload: UpdateFeatureInput = {
        title: this.featureForm.value.title,
        description: this.featureForm.value.description,
        priority: this.featureForm.value.priority,
        epicLink: this.featureForm.value.epicLink
      };

      this.featuresService.apiProjectsProjectIdFeaturesUpdateFeatureFeatureIdPut(this.projectId, this.featureData.id, updatePayload)
        .subscribe({
          next: (response) => this.handleSuccess(response),
          error: (err) => this.handleError(err)
        });
    } else {
      // Prepare CreateFeatureInput mapping for create.
      const createPayload: CreateFeatureInput = {
        title: this.featureForm.value.title,
        description: this.featureForm.value.description,
        priority: this.featureForm.value.priority,
        epicLink: this.featureForm.value.epicLink
      };

      this.featuresService.apiProjectsProjectIdFeaturesCreateNewFeaturePost(this.projectId, createPayload)
        .subscribe({
          next: (response) => this.handleSuccess(response),
          error: (err) => this.handleError(err)
        });
    }
  }

  private handleSuccess(response: any): void {
    this.messageService.add({
      severity: 'success',
      summary: 'Success',
      detail: this.featureData
        ? 'Feature updated successfully'
        : 'Feature created successfully'
    });
    this.isSubmitting.set(false);
    this.onClose.emit(true);
  }

  private handleError(err: any): void {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail: this.featureData
        ? 'Failed to update feature'
        : 'Failed to create feature'
    });
    this.isSubmitting.set(false);
    this.onClose.emit(false);
  }

  cancel(): void {
    this.onClose.emit(null);
  }
}