using System.Text.RegularExpressions;

namespace DisJockey.BotService;

public partial class RegexHelpers
{
    public static string StripSpecialCharacters(string input) => MatchSpecialCharacters().Replace(input, string.Empty);

    [GeneratedRegex(@"[^a-zA-Z0-9 ]")]
    private static partial Regex MatchSpecialCharacters();
}
