namespace Cdr.Api.Models.Pagination;

public class PagedRequestOrder
{
    public required string ColumnName { get; set; }

    public required bool DirectionDesc { get; set; }
}