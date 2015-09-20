namespace Common.Querying
{
    public interface ISortingCriteria
    {
        string SortColunName { get; }
        bool Ascending { get; }
    }
}
