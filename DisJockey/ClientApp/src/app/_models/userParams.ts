import { PaginationType } from "../_enums/paginationType";
import { Pagination, YouTubePagination } from "./pagination";

export class UserParams {
    pageNumber: number = 1;
    pageSize: number = 10;

    query: string = "";
    sortBy: string = "";

    pageToken: string = "";

    constructor(query?: string) {
        this.query = query;
    }

    incrementPage(pagination: Pagination) {
        switch (pagination.paginationType) {
            case (PaginationType.DisJockey):
                this.pageNumber++;
                break;
            case (PaginationType.YouTubeApi):
                const youTubePagination = pagination as YouTubePagination;
                this.pageToken = youTubePagination.nextPageToken;
                break;
            default:
                throw "Invalid Pagination Type";
        }
    }
}
