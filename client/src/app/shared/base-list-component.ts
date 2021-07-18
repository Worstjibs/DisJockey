import { Observable } from "rxjs";
import { PaginatedResult, Pagination } from "../_models/pagination";
import { UserParams } from "../_models/userParams"

export abstract class BaseListComponent<T> {
    userParams: UserParams;
    pagination: Pagination;
    results: T[];

    constructor() {
        this.results = [];
        this.resetUserParams();
    }

    loadData(): void {
        this.loadServiceData().subscribe((response: PaginatedResult<T>) => {
            this.pagination = response.pagination;
            this.results = this.results.concat(response.result);
        });
    }

    resetUserParams(): void {
        this.userParams = new UserParams();
    }

    abstract loadServiceData(): Observable<PaginatedResult<T>>;

}