import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseListComponent } from '../../shared/base-list-component';
import { PaginatedResult } from '../../_models/pagination';
import { Playlist } from '../../_models/playlist';
import { Track } from '../../_models/track';
import { UserParams } from '../../_models/userParams';
import { PlaylistsService } from '../../_services/playlists.service';
import { TracksService } from '../../_services/tracks.service';

@Component({
	selector: 'app-member-playlists-track-list',
	templateUrl: './member-playlists-track-list.component.html',
	styleUrls: ['./member-playlists-track-list.component.css']
})
export class MemberPlaylistsTrackListComponent extends BaseListComponent<Track> {
	private _playlist: Playlist;

	@Input()
	set playlist(playlist: Playlist) {
		this._playlist = playlist;
		this.resetUserParams();
	}

	@Output() trackPlayed = new EventEmitter<{ track: Track, playNow: boolean }>();

	constructor(
		private readonly playlistsService: PlaylistsService,
		private readonly tracksService: TracksService
	) {
		super();
	}

	playTrack(track: Track) {
		this.tracksService.playTrack(track, true);
	}

	queueTrack(track: Track) {
		this.tracksService.playTrack(track, false);
	}

	loadServiceData(): Observable<PaginatedResult<Track>> {
		return this.playlistsService.getPlaylistTracks(this.userParams, this._playlist.youtubeId);
	}

}
