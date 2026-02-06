class PagedResult<T> implements PagedResultModel<T> {
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  items: T[];

  constructor(data: PagedResultModel<T>) {
    this.pageIndex = data.pageIndex;
    this.pageSize = data.pageSize;
    this.totalCount = data.totalCount;
    this.totalPages = data.totalPages;
    this.items = data.items ?? [];
  }
}
