import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { Track } from '../_models/track';
import { Observable, of } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class TracksService {
    baseUrl = environment.apiUrl;
    tracks: Track[] = [];

    constructor(private http: HttpClient) { }

    getTracks() {
        if (this.tracks.length > 0) return of(this.tracks);

        return this.http.get<Track[]>(this.baseUrl + 'track').pipe(
            map(tracks => {
                this.tracks = tracks;
                return tracks;
            })
        );
    }
}
