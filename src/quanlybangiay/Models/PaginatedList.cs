using Microsoft.EntityFrameworkCore;

namespace quanlybangiay.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = pageSize > 0 ? (int)Math.Ceiling(count / (double)pageSize) : 1;
            AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();

            if (pageSize <= 0)
            {
                var all = await source.ToListAsync();
                return new PaginatedList<T>(all, count, 1, 0);
            }

            pageIndex = Math.Max(1, pageIndex);
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (pageIndex > totalPages && totalPages > 0) pageIndex = totalPages;

            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
