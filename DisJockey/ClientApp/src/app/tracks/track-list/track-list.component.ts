import { Component, Input, OnInit, ViewChildren } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseListComponent } from '../../shared/base-list-component';
import { TrackLikeEvent } from '../../_events/trackLikeEvent';
import { TrackPlayEvent } from '../../_events/trackPlayEvent';
import { Member } from '../../_models/member';
import { PaginatedResult } from '../../_models/pagination';
import { Playlist } from '../../_models/playlist';
import { Track } from '../../_models/track';
import { PlaylistsService } from '../../_services/playlists.service';
import { TracksService } from '../../_services/tracks.service';
import { TrackItemComponent } from '../track-item/track-item.component';

@Component({
	selector: 'app-track-list',
	templateUrl: './track-list.component.html',
	styleUrls: ['./track-list.component.css']
})
export class TrackListComponent extends BaseListComponent<Track> implements OnInit {
	@ViewChildren('trackItem') trackItemComponents: TrackItemComponent[];

	@Input() member: Member;
	@Input() innerContainer: boolean;

	_playlist: Playlist;
	@Input() set playlist(playlist: Playlist) {
		let loadPlaylistData = this._playlist ? true : false;

		this._playlist = playlist;

		if (loadPlaylistData) {
			this.resetUserParams();
        }		
	}

	scrollTimeout: boolean;

	constructor(
		private readonly tracksService: TracksService,
		private readonly playlistsService: PlaylistsService
	) {
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
		}, 600);
	}

	onScroll(event): void {
		if (!this.scrollTimeout) {
			if (event.target.offsetHeight + event.target.scrollTop >= event.target.scrollHeight) {
				this.loadMore();

				this.scrollTimeout = true;
				window.setTimeout(() => { this.scrollTimeout = false; }, 50);
			}
		}
	}

	loadServiceData(): Observable<PaginatedResult<Track>> {
		if (this.member) {
			return this.tracksService.getTrackPlaysForMember(this.userParams, this.member.discordId);
		} else if (this._playlist) {
			return this.playlistsService.getPlaylistTracks(this.userParams, this._playlist.youtubeId);
		}

		return this.tracksService.getTracks(this.userParams);
	}

}
