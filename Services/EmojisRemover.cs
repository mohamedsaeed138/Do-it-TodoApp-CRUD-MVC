using System.Text.RegularExpressions;

public class EmojiRemover : IEmojisRemover
{
    private readonly Regex _emojiRegex = new(
        "[^\u1F600-\u1F6FF\\s]",
        options: RegexOptions.Compiled
    );

    public string RemoveEmojis(string text) => _emojiRegex.Replace(text, "");
}
