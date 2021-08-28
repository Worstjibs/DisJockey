import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.prod';
import { PullUp } from '../_models/pullUp';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
	providedIn: 'root'
})
export class PullupsService {
	baseUrl = environment.production + 'pullups';

	constructor(private readonly http: HttpClient) { }

	getPullupsForMember(userParams: UserParams, discordId: string): Observable<PaginatedResult<PullUp>> {
        let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize, userParams.sortBy);

		return getPaginatedResult<PullUp>(`${this.baseUrl}/${discordId}`, params, this.http);
	}
}
