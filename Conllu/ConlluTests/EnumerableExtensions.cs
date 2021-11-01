using System.Collections.Generic;
using System.Linq;

namespace Conllu.Extensions
{
    internal static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? list)
            => list == null || !list.Any();

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? list)
            => list ?? Enumerable.Empty<T>();

        public static IEnumerable<T>? WhereNotNull<T>(this IEnumerable<T?>? list)
            => list?.Where(x => x != null).Cast<T>();
    }
}