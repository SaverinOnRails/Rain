namespace Compiler;

internal sealed class Lexer
{
    internal required string Source { get; set; }
    private int _index = 0;
    private List<Token> Tokens { get; } = new();
    private int Line = 1;
    private int Column = 0;

    //reset every iteration of the tokenizer
    private int StartLine = 0;
    private int StartColumn = 0;
    public List<Token> Tokenize()
    {
        while (CheckBounds)
        {
            SkipCommentsAndWhiteSpace();
            if (_index >= Source.Length) break;
            var start = _index;
            StartLine = Line;
            StartColumn = Column;
            var el = Advance();
            switch (el)
            {
                case >= 'a' and <= 'z':
                case >= 'A' and <= 'Z':
                case '_':
                    Identifier(start);
                    break;
                case >= '0' and <= '9':
                    Number(start);
                    break;
                case '{':
                    Add(Tag.LeftBrace, start);
                    break;
                case '}':
                    Add(Tag.RightBrace, start);
                    break;
                case '(':
                    Add(Tag.LeftBracket, start);
                    break;
                case ')':
                    Add(Tag.RightBracket, start);
                    break;
                case '=':
                    Add(Tag.Equal, start);
                    break;
                case '+':
                    Add(Tag.Plus, start);
                    break;
                case ';':
                    Add(Tag.Semicolon, start);
                    break;
                default:
                    DiagnosticsPrinter.PrintDiagnostic($"Invalid token {el} at {Line},{Column}");
                    break;
            }
        }

        StartLine = Line;
        StartColumn = Column;
        Add(Tag.EOF, Source.Length);
        return Tokens;
    }
    private void Add(Tag tag, int start)
    {
        Token token = new(tag, start: start, end: _index, startLine: StartLine, startCol: StartColumn, endLine: Line, endCol: Column);
        Tokens.Add(token);
    }
    private void Number(int start)
    {
        if (Source[start] == '0' && CheckBounds && (Source[_index] == 'x' || Source[_index] == 'X'))
        {
            Advance();
            var digits = 0;
            while (CheckBounds)
            {
                var c = Source[_index];
                if (Uri.IsHexDigit(c))
                {
                    digits += 1;
                    Advance();
                }
                else if (c == '_') Advance();
                else break;
            }
            if (digits == 0) DiagnosticsPrinter.PrintDiagnostic($"Invalid Hex number at ({Line},{Column})");
            Add(Tag.IntegerLiteral, start);
            return;
        }
        if (Source[start] == '0' && CheckBounds && (Source[_index] == 'b' || Source[_index] == 'B'))
        {
            Advance();
            var digits = 0;
            while (CheckBounds)
            {
                var c = Source[_index];
                if (c == '0' || c == '1')
                {
                    digits += 1;
                    Advance();
                }
                else if (c == '_') Advance();
                else if (char.IsAsciiLetterOrDigit(c)) DiagnosticsPrinter.PrintDiagnostic($"Invalid binary literal at ({Line},{Column})");
                else break;
            }
            if (digits == 0) DiagnosticsPrinter.PrintDiagnostic($"Invalid binary literal at ({Line},{Column})");
            Add(Tag.IntegerLiteral, start);
            return;
        }
        var is_float = false;
        while (CheckBounds && (char.IsAsciiDigit(Source[_index]) || Source[_index] == '_')) Advance();
        if (CheckBounds && Source[_index] == '.' && (_index + 1 < Source.Length && Source[_index + 1] != '.'))
        {
            is_float = true;
            Advance();
            while (CheckBounds && (char.IsAsciiDigit(Source[_index]) || Source[_index] == '_')) Advance();
        }
        if (CheckBounds && (Source[_index] == 'e' || Source[_index] == 'E'))
        {
            is_float = true;
            Advance();
            if (CheckBounds && (Source[_index] == '+' || Source[_index] == '-')) Advance();
            var exp_digits = 0;
            while (CheckBounds && (char.IsAsciiDigit(Source[_index]) || Source[_index] == '_'))
            {
                if (char.IsAsciiDigit(Source[_index])) exp_digits++;
                Advance();
            }
            if (exp_digits == 0) DiagnosticsPrinter.PrintDiagnostic($"Invalid float exponent ({Line},{Column})");
        }
        if (CheckBounds && Source[_index] == 'f')
        {
            Advance();
            is_float = true;
        }
        else if (CheckBounds && char.IsAsciiLetter(Source[_index])) DiagnosticsPrinter.PrintDiagnostic($"Invalid numeral suffix ({Line},{Column})");
        Add(is_float ? Tag.FloatLiteral : Tag.IntegerLiteral, start);
    }

    private bool CheckBounds => _index < Source.Length;

    private char Advance()
    {
        char c = Source[_index++];

        if (c == '\n')
        {
            Line++;
            Column = 1;
        }
        else
        {
            Column++;
        }
        return c;
    }
    private void Identifier(int start)
    {
        while (CheckBounds &&
                (Source[_index] == '_'
                || char.IsAsciiLetterOrDigit(Source[_index])
             )) Advance();
        Add(Token.KeywordOrIdentifier(Source[start.._index]), start);
    }

    private void SkipCommentsAndWhiteSpace()
    {
        while (CheckBounds)
        {
            var el = Source[_index];
            if (char.IsWhiteSpace(el))
            {
                Advance();
            }
            else if (el == '/')
            {
                if (_index + 1 >= Source.Length) return;
                if (Source[_index + 1] == '/')
                {
                    Advance();
                    Advance();
                    while (CheckBounds && Source[_index] != '\n') Advance();
                }
                else return;
            }
            else return;
        }
    }
}
