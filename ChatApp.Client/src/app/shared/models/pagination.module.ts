export interface Pagination {
  currentPage: number;
  itemPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult<T> {
  items?: T;
  pagination?: Pagination;
}