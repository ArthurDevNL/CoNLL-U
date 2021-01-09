namespace Conllu.Extensions
{
    internal static class StringExtensions
    {
        public static string ValueOrNull(this string x)
            => x == "_" ? null : x;
    }
}