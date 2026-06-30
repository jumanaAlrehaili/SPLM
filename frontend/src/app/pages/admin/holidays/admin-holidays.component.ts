import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TableModule } from 'primeng/table';
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { DatePicker } from 'primeng/datepicker';
import { Select } from 'primeng/select';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';

import { HolidaysService } from '../../../api';
import { CreateHolidayInput } from '../../../api/model/createHolidayInput';
import { UpdateHolidayInput } from '../../../api/model/updateHolidayInput';

@Component({
  selector: 'app-admin-holidays',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    Button,
    Dialog,
    InputText,
    DatePicker,
    Select,
    ToastModule,
    TooltipModule
  ],
  templateUrl: './admin-holidays.component.html',
  styleUrl: './admin-holidays.component.scss'
})
export class AdminHolidaysComponent implements OnInit {
  private holidaysService = inject(HolidaysService);
  private messageService  = inject(MessageService);

  // ── State ──
  holidays = signal<any[]>([]);
  loading  = signal(false);

  selectedYear = signal<number>(new Date().getFullYear());
  yearOptions: { label: string; value: number }[] = [];

  // ── Dialog ──
  displayDialog = signal(false);
  editingHoliday = signal<any | null>(null);
  saving = signal(false);

  formName = '';
  formStartDate: Date | null = null;
  formEndDate: Date | null = null;

  ngOnInit(): void {
    const current = new Date().getFullYear();
    // Offer a range from 5 years back to 5 years ahead.
    for (let y = current - 5; y <= current + 5; y++) {
      this.yearOptions.push({ label: `${y}`, value: y });
    }
    this.loadHolidays();
  }

  onYearChange(year: number): void {
    this.selectedYear.set(year);
    this.loadHolidays();
  }

  loadHolidays(): void {
    this.loading.set(true);
    this.holidaysService.getHolidaysByYear(this.selectedYear()).subscribe({
      next: (data: any) => {
        const list = data?.items ?? data ?? [];
        // Sort by start date ascending for a predictable calendar order.
        this.holidays.set([...list].sort((a, b) =>
          new Date(a.startDate).getTime() - new Date(b.startDate).getTime()
        ));
        this.loading.set(false);
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.message || 'Failed to load holidays'
        });
        this.loading.set(false);
      }
    });
  }

  // ── Create / Edit ──
  openCreateDialog(): void {
    this.editingHoliday.set(null);
    this.formName = '';
    this.formStartDate = null;
    this.formEndDate = null;
    this.displayDialog.set(true);
  }

  openEditDialog(holiday: any): void {
    this.editingHoliday.set(holiday);
    this.formName = holiday.name ?? '';
    this.formStartDate = holiday.startDate ? new Date(holiday.startDate) : null;
    this.formEndDate = holiday.endDate ? new Date(holiday.endDate) : null;
    this.displayDialog.set(true);
  }

  saveHoliday(): void {
    if (!this.formName.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Name is required' });
      return;
    }
    if (!this.formStartDate || !this.formEndDate) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Start and end dates are required' });
      return;
    }
    if (this.formEndDate < this.formStartDate) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'End date cannot be before the start date' });
      return;
    }

    this.saving.set(true);
    const payload = {
      name: this.formName.trim(),
      startDate: this.toDateString(this.formStartDate),
      endDate: this.toDateString(this.formEndDate)
    };

    const editing = this.editingHoliday();
    if (editing) {
      this.holidaysService.updateHoliday(editing.id, payload as UpdateHolidayInput).subscribe({
        next: () => this.onSaveSuccess('Holiday updated'),
        error: (err) => this.onSaveError(err, 'Failed to update holiday')
      });
    } else {
      this.holidaysService.createHoliday(payload as CreateHolidayInput).subscribe({
        next: () => this.onSaveSuccess('Holiday created'),
        error: (err) => this.onSaveError(err, 'Failed to create holiday')
      });
    }
  }

  deleteHoliday(holiday: any): void {
    if (!confirm(`Delete "${holiday.name}" permanently?`)) return;

    this.loading.set(true);
    this.holidaysService.deleteHoliday(holiday.id).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Holiday deleted' });
        this.loadHolidays();
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.message || 'Failed to delete holiday'
        });
        this.loading.set(false);
      }
    });
  }

  // ── Helpers ──
  private onSaveSuccess(detail: string): void {
    this.messageService.add({ severity: 'success', summary: 'Success', detail });
    this.displayDialog.set(false);
    this.saving.set(false);
    // If the saved start date moved to a different year, switch the view to it.
    if (this.formStartDate && this.formStartDate.getFullYear() !== this.selectedYear()) {
      this.onYearChange(this.formStartDate.getFullYear());
    } else {
      this.loadHolidays();
    }
  }

  private onSaveError(err: any, fallback: string): void {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail: err.error?.message || fallback
    });
    this.saving.set(false);
  }

  /** Formats a Date as YYYY-MM-DD in local time (avoids timezone shifting the day). */
  private toDateString(d: Date): string {
    const y = d.getFullYear();
    const m = `${d.getMonth() + 1}`.padStart(2, '0');
    const day = `${d.getDate()}`.padStart(2, '0');
    return `${y}-${m}-${day}`;
  }

  /** Inclusive number of days the holiday spans. */
  getDayCount(holiday: any): number {
    if (!holiday?.startDate || !holiday?.endDate) return 0;
    const start = new Date(holiday.startDate);
    const end = new Date(holiday.endDate);
    const ms = end.getTime() - start.getTime();
    if (isNaN(ms) || ms < 0) return 1;
    return Math.floor(ms / 86_400_000) + 1;
  }
}
