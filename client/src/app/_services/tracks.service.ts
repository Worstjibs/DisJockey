import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map, take } from 'rxjs/operators';
import { Track } from '../_models/track';
import { Observable, of } from 'rxjs';
import { UserParams } from '../_models/userParams';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
    providedIn: 'root'
})
export class TracksService {
    baseUrl = environment.apiUrl;
    tracks: Track[] = [];
    userParams : UserParams;

    constructor(private http: HttpClient) { 
        this.userParams = new UserParams();
    }

    getTracks(userParams: UserParams) {
        // if (this.tracks.length > 0) return of(this.tracks);

        let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

        return getPaginatedResult<Track[]>(this.baseUrl + 'track', params, this.http);

        // return this.http.get<Track[]>(this.baseUrl + 'track').pipe(
        //     map(tracks => {
        //         this.tracks = tracks;
        //         return tracks;
        //     })
        // );
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

    playTrack(track: Track) {
        return this.http.post(this.baseUrl + 'track/play', {'YoutubeId': track.youtubeId}).subscribe(response => {
            console.log(response);
        });
    }

    getUserParams() {
        return this.userParams;
    }

    setUserParams(userParams: UserParams) {
        this.userParams = userParams;
    }
}
