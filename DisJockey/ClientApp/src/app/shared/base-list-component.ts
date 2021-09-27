import { Observable } from "rxjs";
import { PaginatedResult, Pagination } from "../_models/pagination";
import { UserParams } from "../_models/userParams"

export abstract class BaseListComponent<T> {
    userParams: UserParams;
    pagination: Pagination;
    results: T[];

    get noMoreResults() {
        return this.pagination.noMoreResults();
    }

    loadData(): void {
        this.loadServiceData().subscribe((response: PaginatedResult<T>) => {
            this.pagination = response.pagination;
            this.results = this.results.concat(response.result);
        });
    }

    resetUserParams(userParams?: UserParams): void {
        if (userParams) {
            this.userParams = userParams;
        } else {
            this.userParams = new UserParams();
        }        

        this.results = [];

        this.loadData();
    }

    sortBy(predicate: string): void {
        let userParams = new UserParams();
        userParams.sortBy = predicate;

        this.resetUserParams(userParams);
    }

    loadMore(): void {
        if (!this.noMoreResults) {
            this.userParams.incrementPage(this.pagination);

            this.loadData();
        }
    }

    abstract loadServiceData(): Observable<PaginatedResult<T>>;

}