import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.prod';
import { Playlist } from '../_models/playlist';

@Injectable({
	providedIn: 'root'
})
export class PlaylistsService {
	baseUrl = environment.apiUrl + 'playlists';

	constructor(private readonly http: HttpClient) { }

	getPlaylistByYoutubeId(youtubeId: string): Observable<Playlist> {
		return this.http.get<Playlist>(`${this.baseUrl}/${youtubeId}`);
	}
}
