import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-bottom-nav',
  imports: [],
  templateUrl: './bottom-nav.html',
  styleUrl: './bottom-nav.scss',
})
export class BottomNav implements OnInit, OnDestroy {
  private readonly hubUrl = '/hubs/track-control';
  private hubConnection: signalR.HubConnection;

  protected readonly message = signal<string | null>(null);

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('SendMessageAsync', (value: string) => {
      this.message.set(value);
    });
  }

  async ngOnInit(): Promise<void> {
    await this.hubConnection.start();
  }

  async ngOnDestroy(): Promise<void> {
    await this.hubConnection?.stop();
  }
}
