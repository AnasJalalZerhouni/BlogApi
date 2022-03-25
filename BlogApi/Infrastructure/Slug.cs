using System.Text.RegularExpressions;

namespace BlogApi.Infrastructure
{
    public static class Slug
    {
        public static string? GenerateSlug(this string? phrase)
        {
            if (phrase is null)
            {
                return null;
            }

            string str = phrase.ToLowerInvariant();

            //remove Invalide Chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();

            //cut and trim
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            // hyphens
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }
    }
}
