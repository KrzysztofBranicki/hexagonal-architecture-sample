namespace Common.Querying
{
    public interface IPageCriteria
    {
        int PageNumber { get; }
        int PageSize { get; }
    }
}
