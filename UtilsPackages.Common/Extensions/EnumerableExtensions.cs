namespace UtilsPackages.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<DuplicatedItem<T>> FindDuplicated<T>(this IEnumerable<T> source)
        {
            return source
                .GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => new DuplicatedItem<T> { Item = x.Key, Count = x.Count() });
        }
    }

    public class DuplicatedItem<T>
    {
        public T Item { get; init; }
        public int Count { get; init; }
    }
}
