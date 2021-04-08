import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map, take } from 'rxjs/operators';
import { Track } from '../_models/track';
import { of } from 'rxjs';

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

    postTrackLike(track: Track, liked: boolean) {        
        const trackLikeDto = {
            youtubeId: track.youtubeId,
            liked: liked
        }

        return this.http.post(this.baseUrl + 'track/like', trackLikeDto).subscribe(() => {
            if (track.likedByUser !== liked && track.likedByUser !== null) {
                if (liked) {
                    track.dislikes--;
                } else {
                    track.likes--;
                }
            }

            if (liked) {
                track.likes++;
            } else {
                track.dislikes++;
            }
            
            track.likedByUser = liked;    
        });
    }
}
