namespace Compiler;

internal sealed class Token
{
    public static Dictionary<string, Tag> KeywordMap = new()
    {
        {"void",Tag.KeywordVoid},
        {"var",Tag.KeywordVar},
        {"return",Tag.KeywordReturn}
    };
    public Tag Tag { get; }
    public int Start { get; }
    public int End { get; }

    public Token(Tag tag, int start, int end)
    {
        Tag = tag;
        Start = start;
        End = end;
    }

    public static Tag KeywordOrIdentifier(string buf)
    {
        Tag tag;
        var tryget = KeywordMap.TryGetValue(buf, out tag);
        if (tryget) return tag;
        return Tag.Identifier;
    }
}

internal enum Tag
{
    IntegerLiteral,
    FloatLiteral,
    Identifier,
    KeywordVoid,
    KeywordReturn,
    KeywordVar,

    EOF,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    Equal,
    Plus,
    Semicolon,
}

