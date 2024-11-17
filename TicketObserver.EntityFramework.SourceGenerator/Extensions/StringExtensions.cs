namespace TicketObserver.EntityFramework.SourceGenerator.Extensions;

public static class StringExtensions
{
    public static string ConvertToPascalCase(this string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;

        int startIndex = 0;
        if (s.StartsWith("_"))
        {
            if (s.Length == 1) return s;
            
            startIndex = 1;
        }
        
        return char.ToUpper(s[startIndex]) + s.Substring(startIndex + 1);
    }
}