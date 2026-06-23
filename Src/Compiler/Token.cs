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

    public int StartLine { get; }
    public int StartColumn { get; }
    public int EndLine { get; }
    public int EndColumn { get; }

    public Token(Tag tag, int start, int end, int startLine, int startCol, int endLine, int endCol)
    {
        Tag = tag;
        Start = start;
        End = end;
        StartLine = startLine;
        StartColumn = startCol;
        EndLine = endLine;
        EndColumn = endCol;
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

