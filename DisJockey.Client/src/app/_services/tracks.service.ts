import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map, take } from 'rxjs/operators';
import { Track } from '../_models/track';
import { Observable, of } from 'rxjs';
import { UserParams } from '../_models/userParams';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { PaginatedResult } from '../_models/pagination';
import { ToastrService } from 'ngx-toastr';
import { PaginationType } from '../_enums/paginationType';

@Injectable({
    providedIn: 'root'
})
export class TracksService {
    baseUrl = environment.apiUrl + 'tracks';
    userParams : UserParams;

    constructor(private readonly http: HttpClient, private readonly toastr: ToastrService) { 
        this.userParams = new UserParams();
    }

    getTracks(userParams: UserParams): Observable<PaginatedResult<Track>> {
        return this.getPaginatedTrackResult(userParams);
    }

    getTrackPlaysForMember(userParams: UserParams, discordId: string): Observable<PaginatedResult<Track>> {
        return this.getPaginatedTrackResult(userParams, discordId);
    }

    private getPaginatedTrackResult(userParams: UserParams, discordId?: string): Observable<PaginatedResult<Track>> {
        let url = this.baseUrl;

        if (discordId) {
            url += `/${discordId}`;
		}

        let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize, userParams.sortBy);

        return getPaginatedResult<Track>(url, params, this.http, PaginationType.DisJockey);
    }

    postTrackLike(track: Track, liked: boolean) {        
        const trackLikeDto = {
            youtubeId: track.youtubeId,
            liked: liked
        }

        return this.http.post(this.baseUrl + '/like', trackLikeDto).subscribe(() => {
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

    playTrack(track: Track, playNow: boolean) {
        let trackPlayDto = {
            'YoutubeId': track.youtubeId,
            'PlayNow': playNow
        }

        return this.http.post(this.baseUrl + '/play', trackPlayDto).subscribe(() => {
            const message = playNow ? "is now playing" : "has been queued";

            this.toastr.success(`${track.title} ${message}`);
        });
    }

    getUserParams() {
        return this.userParams;
    }

    setUserParams(userParams: UserParams) {
        this.userParams = userParams;
    }

    resetUserParams(): UserParams {
        this.userParams = new UserParams();
        return this.userParams;
    }
}
