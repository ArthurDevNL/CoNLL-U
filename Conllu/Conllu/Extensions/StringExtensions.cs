namespace Conllu.Extensions
{
    public static class StringExtensions
    {
        public static string ValueOrNull(this string x)
            => x == "_" ? null : x;
    }
}