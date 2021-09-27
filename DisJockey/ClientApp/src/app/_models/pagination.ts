import { PaginationType } from "../_enums/paginationType";

export interface Pagination {
    paginationType: PaginationType;
    noMoreResults(): boolean;
}

export class DisJockeyPagination implements Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;

    constructor(json: string) {
        const params = JSON.parse(json) as DisJockeyPagination;

        this.currentPage = params.currentPage;
        this.itemsPerPage = params.itemsPerPage;
        this.totalItems = params.totalItems;
        this.totalPages = params.totalPages;
    }

    noMoreResults() {
        return this.currentPage === this.totalPages;
    }
    
    paginationType = PaginationType.DisJockey;
}

export class YouTubePagination implements Pagination {
    currentPageToken: string;
    nextPageToken: string;
    previousPageToken: string;

    constructor(json: string) {
        const params = JSON.parse(json) as YouTubePagination;

        this.currentPageToken = params.currentPageToken;
        this.nextPageToken = params.nextPageToken;
        this.previousPageToken = params.previousPageToken;
    }

    noMoreResults(): boolean {
        return this.nextPageToken === "";
    };

    paginationType = PaginationType.YouTubeApi;
}

export class PaginatedResult<T> {
    pagination: Pagination;
    result: T[];
}