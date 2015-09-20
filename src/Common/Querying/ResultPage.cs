using System;
using System.Collections.Generic;
using System.Linq;

using static System.Math;

namespace Common.Querying
{
    public class ResultPage<T>
    {
        public IEnumerable<T> Items { get; }
        public int PageSize { get; }
        public int PageNumber { get; }
        public int TotalItemsCount { get; }

        public int PagesCount => (int)Ceiling((double)TotalItemsCount / PageSize);

        public ResultPage(IEnumerable<T> items, int pageNumber, int pageSize, int totalItemsCount)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (pageSize <= 0) throw new ArgumentException(nameof(pageSize));

            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItemsCount = totalItemsCount;
        }

        public static ResultPage<T> CreateNoItemsResult(int pageNumber, int pageSize, int totalItemsCount)
        {
            return new ResultPage<T>(Enumerable.Empty<T>(), pageNumber, pageSize, totalItemsCount);
        }
    }
}
