import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { PaginationType } from "../_enums/paginationType";
import { DisJockeyPagination, PaginatedResult, YouTubePagination } from "../_models/pagination";

export function getPaginatedResult<T>(url, params: HttpParams, http: HttpClient, paginationType: PaginationType): Observable<PaginatedResult<T>> {
    const paginatedResult = new PaginatedResult<T>();

    return http.get<T[]>(url, { observe: 'response', params }).pipe(
        map(response => {
            const result = response.body as any;

            if (paginationType == PaginationType.DisJockey) {
                paginatedResult.result = result.items;
                paginatedResult.pagination = new DisJockeyPagination(result.currentPage, result.itemsPerPage, result.totalItems, result.totalPages);

                return paginatedResult;
            }

            if (paginationType == PaginationType.YouTubeApi) {
                const paginationParams = response.headers.get('Pagination');

                paginatedResult.result = response.body;
                paginatedResult.pagination = new YouTubePagination(paginationParams);

                return paginatedResult;
            }

            throw "Invalid Pagination Type";
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