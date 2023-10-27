import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { PaginationType } from "../_enums/paginationType";
import { DisJockeyPagination, PaginatedResult, YouTubePagination } from "../_models/pagination";

export function getPaginatedResult<T>(url, params: HttpParams, http: HttpClient, paginationType: PaginationType): Observable<PaginatedResult<T>> {
    const paginatedResult = new PaginatedResult<T>();

    return http.get<T[]>(url, { observe: 'response', params }).pipe(
        map(response => {
            paginatedResult.result = response.body;

            const paginationParams = response.headers.get('Pagination');
            if (paginationParams) {
                switch (paginationType) {
                    case PaginationType.DisJockey:
                        paginatedResult.pagination = new DisJockeyPagination(paginationParams);
                        break;
                    case PaginationType.YouTubeApi:
                        paginatedResult.pagination = new YouTubePagination(paginationParams);
                        break;
                    default:
                        throw "Invalid Pagination Type";
                }
                
            }
            return paginatedResult;
        })
    );
}

export function getPaginationHeaders(pageNumber: number, pageSize: number, sortBy: string) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    params = params.append('sortBy', sortBy);

    return params;
}

export function getYouTubePaginationHeaders(query: string, pageToken: string) {
    let params = new HttpParams();

    params = params.append('query', query);
    params = params.append('pageToken', pageToken);

    return params;
}