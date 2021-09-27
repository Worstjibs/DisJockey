import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { UserParams } from '../_models/userParams';
import { getPaginatedResult, getYouTubePaginationHeaders } from '../_helper/paginationHelper';
import { Track } from '../_models/track';
import { PaginationType } from '../_enums/paginationType';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
    providedIn: 'root'
})
export class SearchService {
    baseUrl = environment.apiUrl + 'search';

    constructor(private readonly http: HttpClient) { }

    searchTracks(userParams: UserParams): Observable<PaginatedResult<Track>> {
        const params = getYouTubePaginationHeaders(userParams.query, userParams.pageToken);

        return getPaginatedResult<Track>(this.baseUrl, params, this.http, PaginationType.YouTubeApi);
    }
}
