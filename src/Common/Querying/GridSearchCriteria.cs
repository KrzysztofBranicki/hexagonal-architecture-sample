namespace Common.Querying
{
    public class GridSearchCriteria : IPageCriteria, ISortingCriteria
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public string SortColunName { get; }
        public bool Ascending { get; }

        public GridSearchCriteria(int pageNumber, int pageSize, string sortColunName, bool @ascending)
        {
            Ascending = @ascending;
            SortColunName = sortColunName;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }
}
