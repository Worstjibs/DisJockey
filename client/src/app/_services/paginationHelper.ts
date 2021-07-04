import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_models/pagination";
import { Track } from "../_models/track";

export function getPaginatedResult<T>(url, params: HttpParams, http: HttpClient): Observable<PaginatedResult<T>> {
    const paginatedResult = new PaginatedResult<T>();

    return http.get<T>(url, { observe: 'response', params }).pipe(
        map(response => {
            paginatedResult.result = response.body;

            const paginationParams = response.headers.get('Pagination');
            if (paginationParams) {
                paginatedResult.pagination = JSON.parse(paginationParams);
            }
            return paginatedResult;
        })
    );
}

export function getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
}