import { Observable } from "rxjs";
import { UserParams } from "./models/userParams";
import { PaginatedResult, Pagination } from "./models/pagination";

export abstract class BaseListComponent<T> {
    userParams = new UserParams();
    pagination = {} as Pagination;
    results: T[] = [];

    get noMoreResults() {
        return this.pagination?.noMoreResults();
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
        if (!this.noMoreResults && this.pagination) {
            this.userParams?.incrementPage(this.pagination);

            this.loadData();
        }
    }

    abstract loadServiceData(): Observable<PaginatedResult<T>>;

}