using System.Linq;
using System.Linq.Expressions;

namespace Common.Querying
{
    public static class PagingExtensions
    {
        public static ResultPage<T> ApplyGridSearchCriteria<T>(this IQueryable<T> query, GridSearchCriteria criteria)
        {
            return query.OrderBy(criteria).PickPage(criteria);
        }

        public static ResultPage<T> PickPage<T>(this IQueryable<T> query, IPageCriteria criteria)
        {
            return PickPage(query, criteria, query.Count());
        }

        public static ResultPage<T> PickPage<T>(this IQueryable<T> query, IPageCriteria criteria, int totalItemsCount)
        {
            return new ResultPage<T>(query.Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize).ToList(), criteria.PageNumber, criteria.PageSize, totalItemsCount);
        }
        
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, ISortingCriteria sortingCriteria)
        {
            return query.OrderByPropertyName(sortingCriteria.SortColunName, sortingCriteria.Ascending);
        }

        public static IQueryable<T> OrderByPropertyName<T>(this IQueryable<T> query, string propertyName, bool ascending)
        {
            if (string.IsNullOrEmpty(propertyName))
                return query;

            var type = typeof(T);
            var myObject = Expression.Parameter(type, "MyObject");

            var props = propertyName.Split('.');
            
            foreach (var prop in props.Take(props.Length - 1))
            {
                type = type.GetProperty(prop).PropertyType;
            }
            var finalProp = type.GetProperty(props.Last());
            Expression myProperty = Expression.Property(myObject, finalProp);

            var myLamda = Expression.Lambda(myProperty, myObject);
            var method = ascending ? "OrderBy" : "OrderByDescending";
            var types = new[] { query.ElementType, myLamda.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, query.Expression, myLamda);

            return query.Provider.CreateQuery<T>(mce);
        }
    }
}
