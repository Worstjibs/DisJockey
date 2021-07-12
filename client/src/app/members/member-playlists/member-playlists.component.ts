import { Component, Input, OnInit } from '@angular/core';
import { Playlist } from '../../_models/playlist';
import { PlaylistsService } from '../../_services/playlists.service';

@Component({
	selector: 'app-member-playlists',
	templateUrl: './member-playlists.component.html',
	styleUrls: ['./member-playlists.component.css']
})
export class MemberPlaylistsComponent implements OnInit {
	@Input() playlists: Playlist[];
	selectedPlaylist: Playlist;

	constructor(private readonly playlistsService: PlaylistsService) { }

	ngOnInit(): void {
		if (this.playlists.length > 0) {
			this.selectedPlaylist = this.playlists[0]
			this.getPlaylist(this.selectedPlaylist);
		}
	}

	getPlaylist(playlist: Playlist): void {
		if (!playlist.tracks) {
			this.playlistsService.getPlaylistByYoutubeId(playlist.youtubeId)
				.subscribe((response: Playlist) => {
					this.selectedPlaylist.tracks = response.tracks;
				});
		}
	}

	playlistClicked(playlist: Playlist) {
		if (this.selectedPlaylist == playlist) {
			return;
		}

		this.selectedPlaylist = playlist;
		this.getPlaylist(this.selectedPlaylist);
	}

}
