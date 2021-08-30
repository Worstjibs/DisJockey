import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { map, take } from 'rxjs/operators';
import { Track } from 'src/app/_models/track';
import { TracksService } from 'src/app/_services/tracks.service';
import { TrackListComponent } from '../../tracks/track-list/track-list.component';
import { Playlist } from '../../_models/playlist';
import { AccountService } from '../../_services/account.service';
import { PlaylistsService } from '../../_services/playlists.service';

@Component({
	selector: 'app-member-playlists',
	templateUrl: './member-playlists.component.html',
	styleUrls: ['./member-playlists.component.css']
})
export class MemberPlaylistsComponent implements OnInit {
	@ViewChild('trackListComponent') trackListComponent: TrackListComponent;

	@Input() playlists: Playlist[];
	@Input() discordId: string;
	selectedPlaylist: Playlist;

	addPlaylistEnabled: boolean;

	constructor(
		private readonly playlistsService: PlaylistsService,
		private readonly accountService: AccountService,
        private readonly tracksService: TracksService
	) { }

	get isCurrentUser() {
		return this.accountService.currentUser$.pipe(take(1)).pipe(map(user => user.discordId === this.discordId));
	}

	ngOnInit() {
		if (this.playlists.length > 0) {
			this.addPlaylistEnabled = false;
			this.selectedPlaylist = this.playlists[0]
		} else {
			this.addPlaylistEnabled = true;
			this.playlists = [];
		}
	}

	playlistClicked(playlist: Playlist) {
		this.addPlaylistEnabled = false;

		if (this.selectedPlaylist == playlist) {
			return;
		}

		this.selectedPlaylist = playlist;
	}

	enableAddMode() {
		this.addPlaylistEnabled = true;
	}

	addPlaylist(data) {
		this.playlistsService.addPlaylist(data.youtubeId).subscribe((response: Playlist) => {
			if (response) {
				this.playlists.push(response);

				this.addPlaylistEnabled = false;
				this.selectedPlaylist = response;
			}
		});
	}

    playTrack(track: Track) {
        this.tracksService.playTrack(track, true);
    }    

	queueTrack(track: Track) {
		this.tracksService.playTrack(track, false);
	}

	onScroll(event) {
		this.trackListComponent.onScroll(event);
    }

}
