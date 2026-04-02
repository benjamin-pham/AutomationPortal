export interface PagedList<TItem> {
    readonly items: TItem[];
    readonly pageNumber: number;
    readonly pageSize: number;
    readonly totalItems: number;
    readonly totalPages: number;
    readonly hasNextPage: boolean;
    readonly hasPreviousPage: boolean;
}
