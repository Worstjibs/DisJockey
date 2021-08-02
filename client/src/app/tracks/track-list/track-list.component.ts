import { Component, Input, OnInit, ViewChildren } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseListComponent } from '../../shared/base-list-component';
import { TrackLikeEvent } from '../../_events/trackLikeEvent';
import { TrackPlayEvent } from '../../_events/trackPlayEvent';
import { Member } from '../../_models/member';
import { PaginatedResult } from '../../_models/pagination';
import { Track } from '../../_models/track';
import { TracksService } from '../../_services/tracks.service';
import { TrackItemComponent } from '../track-item/track-item.component';

@Component({
	selector: 'app-track-list',
	templateUrl: './track-list.component.html',
	styleUrls: ['./track-list.component.css']
})
export class TrackListComponent extends BaseListComponent<Track> implements OnInit {
	@Input() member: Member;

	@ViewChildren('trackItem') trackItemComponents: TrackItemComponent[];

	constructor(private readonly tracksService: TracksService) {
		super();
	}

	ngOnInit(): void {
		this.resetUserParams();
	}

	likeTrack(event): void {
		var track: Track = event.track;

		if (event.liked != track.likedByUser) {
			this.tracksService.postTrackLike(event.track, event.liked);
		}
	}

	playTrack(event): void {
		if (event.track) {
			this.tracksService.playTrack(event.track, event.playNow);
		}
	}

	trackExpanded(event: Track): void {
		// Close the current track
		const currentTrack = this.trackItemComponents.find(x => x.showDetail);
		if (currentTrack) {
			currentTrack.showDetail = false;
		}

		const requestedTrack = this.trackItemComponents.find(x => x.track.youtubeId == event.youtubeId);

		const element = requestedTrack.elementRef.nativeElement;
		requestedTrack.showDetail = true;

		// Wait for column animation
		window.setTimeout(() => {
			element.scrollIntoView({ behavior: 'smooth', block: 'center' });
		}, 500);
	}

	loadServiceData(): Observable<PaginatedResult<Track>> {
		if (this.member) {
			return this.tracksService.getTrackPlaysForMember(this.userParams, this.member.discordId);
		}

		return this.tracksService.getTracks(this.userParams);
	}

}
