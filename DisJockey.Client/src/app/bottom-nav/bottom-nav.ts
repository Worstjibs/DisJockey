import { Component, computed, OnDestroy, OnInit, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlay, faPause, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { Track } from '../tracks/models/track';

@Component({
  selector: 'app-bottom-nav',
  imports: [FontAwesomeModule],
  templateUrl: './bottom-nav.html',
  styleUrl: './bottom-nav.scss',
})
export class BottomNav implements OnInit, OnDestroy {
  private readonly hubUrl = '/hubs/track-control';
  private hubConnection: signalR.HubConnection;

  protected readonly userStatus = signal<UserStatusMessage | null>(null);
  protected readonly trackStatus = signal<TrackStatusMessage | null>(null);
  protected readonly trackProgress = signal<TrackProgressMessage | null>(null);

  protected readonly progressPercent = computed(() => {
    const progress = this.trackProgress();
    if (!progress || progress.total === 0) { return 0; }
    return (progress.elapsed / progress.total) * 100;
  });

  protected readonly sliderBackground = computed(() => {
    const pct = this.progressPercent();
    return `linear-gradient(to right, white ${pct}%, rgba(255,255,255,0.2) ${pct}%)`;
  });

  protected get playPauseIcon(): IconDefinition {
    const status = this.trackStatus();
    if (!status) {
      return faPlay; // Default icon when no track is playing
    }

    const icon = status.paused ? faPlay : faPause;

    return icon;
  }

  protected onSeek(event: Event): void {
    const positionSeconds = Number((event.target as HTMLInputElement).value);
    this.hubConnection.invoke('SeekTrack', positionSeconds);
  }

  protected togglePlayPause(): void {
    const userStatus = this.userStatus();
    if (!userStatus) {
      return;
    }

    this.hubConnection.invoke('TogglePlayPause')
      .then(() => this.trackStatus.update(status => status ? { ...status, paused: !status.paused } : null));
  }

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('NotifyUserStatus', (notification: UserStatusMessage) => {
      this.userStatus.set(notification);
      this.trackStatus.set(notification?.trackStatusMessage);
    });

    this.hubConnection.on('NotifyTrackStatus', (notification: TrackStatusMessage) => {
      this.trackStatus.set(notification);
      this.trackProgress.set(null);
    });

    this.hubConnection.on('NotifyTrackProgress', (notification: TrackProgressMessage) => {
      this.trackProgress.set(notification);
    });
  }

  async ngOnInit(): Promise<void> {
    await this.hubConnection.start();
  }

  async ngOnDestroy(): Promise<void> {
    await this.hubConnection?.stop();
  }
}

interface UserStatusMessage {
  voiceChannelName: string;
  serverName: string;
  trackStatusMessage: TrackStatusMessage | null;
}

interface TrackStatusMessage {
  trackName: string;
  paused: boolean;
}

interface TrackProgressMessage {
  elapsed: number;
  total: number;
}
