import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class TracksService {
    baseUrl = environment.apiUrl;
    tracks: any = [];

    constructor(private http: HttpClient) { }

    getTracks() {
        return this.http.get(this.baseUrl + 'track').pipe(
            map(tracks => {
                this.tracks = tracks;
                return tracks;
            })
        );
    }
}
