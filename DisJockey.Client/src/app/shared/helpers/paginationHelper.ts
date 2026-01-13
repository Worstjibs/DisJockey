import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { DisJockeyPagination, PaginatedResult, PaginationType, YouTubePagination } from "../models/pagination";

export function getPaginatedResult<T>(
    url: string,
    params: HttpParams,
    http: HttpClient,
    paginationType: PaginationType): Observable<PaginatedResult<T>> {

    return http.get<T[]>(url, { observe: 'response', params }).pipe(
        map(response => {
            const result = response.body as any;

            if (paginationType == PaginationType.DisJockey) {
                const pagination = new DisJockeyPagination(result.currentPage, result.itemsPerPage, result.totalItems, result.totalPages);

                return new PaginatedResult<T>(pagination, result.items);
            }

            if (paginationType == PaginationType.YouTubeApi) {
                const paginationParams = response.headers.get('Pagination');
                const pagination = new YouTubePagination(paginationParams ?? "");

                return new PaginatedResult<T>(pagination, result.results);
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