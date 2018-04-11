namespace Data.Extensions
{
    public static class StringExtensions
    {
        public static string StripAndLower(this string value)
        {
            return value?.ToLower().Replace(" ", string.Empty);
        }
    }
}
