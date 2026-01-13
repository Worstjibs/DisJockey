import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserParams } from '../../shared/models/userParams';
import { PaginatedResult, PaginationType } from '../../shared/models/pagination';
import { Track } from '../models/track';
import { getPaginatedResult, getPaginationHeaders } from '../../shared/helpers/paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class TrackApi {
  private readonly baseUrl = 'api/tracks'

  userParams = new UserParams();

  private readonly http = inject(HttpClient);
  // private readonly toastr = inject(ToastrService);

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

    return this.http.post('/like', trackLikeDto).subscribe(() => {
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

    return this.http.post(`${this.baseUrl}/play`, trackPlayDto).subscribe(() => {
      const message = playNow ? "is now playing" : "has been queued";

      // this.toastr.success(`${track.title} ${message}`);
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

