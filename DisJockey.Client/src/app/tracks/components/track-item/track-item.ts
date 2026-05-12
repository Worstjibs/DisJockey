import { Component, input, output } from '@angular/core';
import { Track } from '../../models/track';
import { CommonModule } from '@angular/common';
import { TimeagoPipe } from 'ngx-timeago';

@Component({
  selector: 'app-track-item',
  imports: [CommonModule, TimeagoPipe],
  templateUrl: './track-item.html',
  styleUrl: './track-item.scss',
})
export class TrackItem {

  track = input<Track | null>(null)
  trackLiked = output<TrackLikeEvent>();
  trackPlayed = output<TrackPlayEvent>();
  trackExpanded = output<Track>();

  showDetail: boolean = false;

  ngOnInit(): void {
    this.showDetail = false;
  }

  likeTrack(track: Track) {
    this.trackLiked.emit({ track, liked: true });
  }

  dislikeTrack(track: Track) {
    this.trackLiked.emit({ track, liked: false });
  }

  playTrack(track: Track, event: MouseEvent) {
    event.stopPropagation();
    this.trackPlayed.emit({ track, playNow: true });
  }

  queueTrack(track: Track) {
    this.trackPlayed.emit({ track, playNow: false });
  }

  toggleDetail() {
    const track = this.track();
    this.showDetail = !this.showDetail;

    if (track) {
      this.trackExpanded.emit(track);
    }
  }
}

export interface TrackLikeEvent {
  track: Track;
  liked: boolean;
}

export interface TrackPlayEvent {
  track: Track;
  playNow: boolean;
}