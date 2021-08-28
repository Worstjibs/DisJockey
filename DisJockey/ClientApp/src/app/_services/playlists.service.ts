import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.prod';
import { PaginatedResult } from '../_models/pagination';
import { Playlist } from '../_models/playlist';
import { Track } from '../_models/track';
import { UserParams } from '../_models/userParams';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
	providedIn: 'root'
})
export class PlaylistsService {
	baseUrl = environment.apiUrl + 'playlists';

	constructor(private readonly http: HttpClient) { }

	getPlaylistTracks(userParams: UserParams, youtubeId: string): Observable<PaginatedResult<Track>> {
		let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize, userParams.sortBy);

		return getPaginatedResult<Track>(`${this.baseUrl}/${youtubeId}`, params, this.http);
	}

	addPlaylist(youtubeId: string): Observable<Playlist> {
		return this.http.post<Playlist>(this.baseUrl, {playlistId: youtubeId});
	}
}
