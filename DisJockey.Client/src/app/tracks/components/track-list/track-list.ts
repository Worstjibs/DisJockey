import { Component, inject, input, OnInit, viewChildren } from '@angular/core';
import { BaseListComponent } from '../../../shared/base-list-component';
import { Track } from '../../models/track';
import { PaginatedResult } from '../../../shared/models/pagination';
import { TrackListType } from '../../enums/trackListType';
import { Observable } from 'rxjs';
import { UserParams } from '../../../shared/models/userParams';
import { TrackApi } from '../../services/track-api';
import { TrackItem } from '../track-item/track-item';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-track-list',
  imports: [CommonModule, TrackItem],
  templateUrl: './track-list.html',
  styleUrl: './track-list.scss',
})
export class TrackList extends BaseListComponent<Track> implements OnInit {
  trackItemComponents = viewChildren<TrackItem>('trackItem');
  // @ViewChildren('trackItem') trackItemComponents: TrackItemComponent[];

  trackListType = input<TrackListType>();

  // @Input() member: Member;
  // @Input() innerContainer: boolean;

  scrollTimeout: boolean = false;

  private readonly trackApi = inject(TrackApi);

  protected tracks = toSignal(this.trackApi.getTracks(this.userParams));

  ngOnInit(): void {
    if (this.trackListType() !== TrackListType.Search) {
      this.resetUserParams();
    }
  }

  likeTrack(event: any): void {
    var track: Track = event.track;

    if (event.liked != track.likedByUser) {
      this.trackApi.postTrackLike(event.track, event.liked);
    }
  }

  playTrack(event: any): void {
    if (event.track) {
      this.trackApi.playTrack(event.track, event.playNow);
    }
  }

  trackExpanded(event: Track): void {
    // Close the current track
    // const currentTrack = this.trackItemComponents.find(x => x.showDetail);
    // if (currentTrack) {
    // 	currentTrack.showDetail = false;
    // }

    // const requestedTrack = this.trackItemComponents.find(x => x.track.youtubeId == event.youtubeId);

    // const element = requestedTrack.elementRef.nativeElement;
    // requestedTrack.showDetail = true;

    // Wait for column animation
    // window.setTimeout(() => {
    // 	element.scrollIntoView({ behavior: 'smooth', block: 'center' });
    // }, 600);
  }

  onScroll(event: any): void {
  	if (!this.scrollTimeout) {
  		if (event.target.offsetHeight + event.target.scrollTop >= event.target.scrollHeight) {
  			this.loadMore();

  			this.scrollTimeout = true;
  			window.setTimeout(() => { this.scrollTimeout = false; }, 50);
  		}
  	}
  }

  search(query: string) {
    const userParams = new UserParams(query);

    this.resetUserParams(userParams);
  }

  loadServiceData(): Observable<PaginatedResult<Track>> {
    return this.trackApi.getTracks(this.userParams);
  }
}