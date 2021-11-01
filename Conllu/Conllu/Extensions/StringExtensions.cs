using System;
using System.Collections.Generic;

namespace Conllu.Extensions
{
    internal static class StringExtensions
    {
        public static string? ValueOrNull(this string? x)
            => x == "_" ? null : x;

        public static string ValueOrUnderscore(this string? x)
            => x?.Trim().IsNullOrEmpty() == false  ? x : "_";

        public static IEnumerable<string> SplitLines(this string x)
            => x.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            );
    }
}