namespace HRCoreSuite.Application.DTOs.Common;

public class PagedResponse<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

    public PagedResponse(IEnumerable<T> data, int page, int pageSize, int totalItems)
    {
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        Data = data;
    }
}