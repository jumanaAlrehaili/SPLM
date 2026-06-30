import { Injectable, Inject, Optional, signal, computed, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { MessageService } from 'primeng/api';
import { BASE_PATH } from '../api/variables';
import { Configuration } from '../api/configuration';
import { NotificationsService } from '../api/api/notifications.service';

export interface AppNotification {
  id?: number | string;
  title?: string;
  message?: string;
  type?: string;
  createdAt?: string;
  isRead?: boolean;
  [key: string]: any;
}

/**
 * Path the SignalR hub is mapped to in the backend (Program.cs):
 *   app.MapHub<NotificationHub>("/hubs/notifications");
 * If your backend uses a different path, change it here.
 */
const HUB_PATH = '/hubs/notifications';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private connection?: signalR.HubConnection;
  private notificationsApi = inject(NotificationsService);

  // ── Reactive state ──
  readonly notifications = signal<AppNotification[]>([]);
  readonly connected = signal(false);
  readonly unreadCount = computed(() =>
    this.notifications().filter(n => !n.isRead).length
  );

  constructor(
    @Optional() @Inject(BASE_PATH) private basePath: string,
    @Optional() private configuration: Configuration,
    private messageService: MessageService
  ) {}

  /** Builds the hub URL from the API base path (token OR Configuration object). */
  private get hubUrl(): string {
    const raw = this.configuration?.basePath || this.basePath || '';
    const base = raw.replace(/\/+$/, '');
    return `${base}${HUB_PATH}`;
  }

  /** Opens the SignalR connection + loads the saved notifications. Safe to call repeatedly. */
  start(): void {
    // Always (re)load the persisted history — cheap and keeps the badge accurate.
    this.loadFromServer();

    // Already connected/connecting — don't open a second socket.
    if (this.connection &&
        this.connection.state !== signalR.HubConnectionState.Disconnected) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => localStorage.getItem('token') ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveNotification', (notification: AppNotification) =>
      this.handleNotification(notification)
    );

    this.connection.onreconnected(() => {
      this.connected.set(true);
      // Re-sync any notifications missed while the socket was down.
      this.loadFromServer();
    });
    this.connection.onclose(() => this.connected.set(false));

    this.connection.start()
      .then(() => this.connected.set(true))
      .catch(err => console.error(`[NotificationService] connection to "${this.hubUrl}" failed:`, err));
  }

  /** Closes the connection (e.g. on logout). */
  stop(): void {
    this.connection?.stop().catch(() => {});
    this.connection = undefined;
    this.connected.set(false);
    this.notifications.set([]);
  }

  /** Loads the persisted notifications from the backend. */
  loadFromServer(): void {
    this.notificationsApi.getMyNotifications().subscribe({
      next: (res: any) => {
        const data = res?.items ?? res ?? [];
        this.notifications.set((data || []).map((r: any) => this.normalize(r)));
      },
      error: () => { /* offline / unauthorized — keep whatever is in memory */ }
    });
  }

  /** Handles a live push: prepend to list (dedup by id) + show a toast. */
  private handleNotification(raw: AppNotification): void {
    const item = { ...this.normalize(raw), isRead: false };

    this.notifications.update(list => {
      // Drop an existing row with the same id so the live push replaces it at the top.
      const deduped = item.id != null ? list.filter(n => n.id !== item.id) : list;
      return [item, ...deduped];
    });

    this.messageService.add({
      severity: this.toSeverity(item.type),
      summary: item.title ?? 'Notification',
      detail: item.message ?? '',
      life: 5000
    });
  }

  /** Marks a single notification read (persists to backend). */
  markRead(id: number | string | undefined): void {
    if (id == null) return;
    this.notificationsApi.markAsRead(Number(id)).subscribe({ error: () => {} });
    this.notifications.update(list => list.map(n => n.id === id ? { ...n, isRead: true } : n));
  }

  /** Marks every notification read (persists each unread one to the backend). */
  markAllRead(): void {
    this.notifications()
      .filter(n => !n.isRead && n.id != null)
      .forEach(n => this.notificationsApi.markAsRead(Number(n.id)).subscribe({ error: () => {} }));

    this.notifications.update(list => list.map(n => ({ ...n, isRead: true })));
  }

  clear(): void {
    this.notifications.set([]);
  }

  /** Normalizes the backend/push payload into a consistent shape. */
  private normalize(raw: any): AppNotification {
    return {
      ...raw,
      id:        raw?.id ?? raw?.notificationId,
      title:     raw?.title ?? raw?.subject ?? 'Notification',
      message:   raw?.message ?? raw?.body ?? raw?.content ?? raw?.text ?? '',
      type:      raw?.type ?? raw?.notificationType,
      createdAt: raw?.createdAt ?? raw?.createdOn ?? raw?.timestamp ?? new Date().toISOString(),
      isRead:    raw?.isRead ?? raw?.read ?? false
    };
  }

  private toSeverity(type?: string): 'success' | 'info' | 'warn' | 'error' {
    switch ((type ?? '').toLowerCase()) {
      case 'success': return 'success';
      case 'warning':
      case 'warn':    return 'warn';
      case 'error':
      case 'danger':  return 'error';
      default:        return 'info';
    }
  }
}
