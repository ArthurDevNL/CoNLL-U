namespace Conllu.Extensions
{
    internal static class StringExtensions
    {
        public static string ValueOrNull(this string x)
            => x == "_" ? null : x;

        public static string ValueOrUnderscore(this string x)
            => x?.Trim().IsNullOrEmpty() == false  ? x : "_";
    }
}