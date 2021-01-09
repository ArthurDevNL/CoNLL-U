using System.Collections.Generic;
using System.Linq;

namespace Conllu.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
            => list == null || !list.Any();

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
            => list ?? Enumerable.Empty<T>();
    }
}