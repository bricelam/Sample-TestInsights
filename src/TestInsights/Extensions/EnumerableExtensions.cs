namespace System.Collections.Generic
{
    internal static class EnumerableExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                source.Add(item);
            }
        }
    }
}
