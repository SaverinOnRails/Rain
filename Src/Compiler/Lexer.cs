namespace Compiler;

internal sealed class Lexer
{
    internal required string Source { get; set; }
    private int _index = 0;
    private List<Token> Tokens { get; } = new();
    public List<Token> Tokenize()
    {
        while (CheckBounds)
        {
            SkipCommentsAndWhiteSpace();
            if (_index >= Source.Length) break;
            var start = _index;
            var el = Source[_index];
            _index++;
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
                    Tokens.Add(new(Tag.LeftBrace, start, _index));
                    break;
                case '}':
                    Tokens.Add(new(Tag.RightBrace, start, _index));
                    break;
                case '(':
                    Tokens.Add(new(Tag.LeftBracket, start, _index));
                    break;
                case ')':
                    Tokens.Add(new(Tag.RightBracket, start, _index));
                    break;
                case '=':
                    Tokens.Add(new(Tag.Equal, start, _index));
                    break;
                case '+':
                    Tokens.Add(new(Tag.Plus, start, _index));
                    break;
                case ';':
                    Tokens.Add(new(Tag.Semicolon, start, _index));
                    break;
                default:
                    throw new Exception($"Invalid token {el}");
            }
        }
        Tokens.Add(new(Tag.EOF, Source.Length, Source.Length));
        return Tokens;
    }

    private void Number(int start)
    {
        if (Source[start] == '0' && CheckBounds && (Source[_index] == 'x' || Source[_index] == 'X'))
        {
            _index++;
            var digits = 0;
            while (CheckBounds)
            {
                var c = Source[_index];
                if (Uri.IsHexDigit(c))
                {
                    digits += 1;
                    _index++;
                }
                else if (c == '_') _index++;
                else break;
            }
            if (digits == 0) throw new Exception("Invalid Hex Number");
            Tokens.Add(new(Tag.IntegerLiteral, start, _index));
            return;
        }
        if (Source[start] == '0' && CheckBounds && (Source[_index] == 'b' || Source[_index] == 'B'))
        {
            _index++;
            var digits = 0;
            while (CheckBounds)
            {
                var c = Source[_index];
                if (c == '0' || c == '1')
                {
                    digits += 1;
                    _index++;
                }
                else if (c == '_') _index++;
                else if (char.IsAsciiLetterOrDigit(c)) throw new Exception("Invalid binary literal");
                else break;
            }
            if (digits == 0) throw new Exception("Invalid binary literal");
            Tokens.Add(new(Tag.IntegerLiteral, start, _index));
            return;
        }
        var is_float = false;
        while (CheckBounds && (char.IsAsciiDigit(Source[_index]) || Source[_index] == '_')) _index++;
        if (CheckBounds && Source[_index] == '.' && (_index + 1 < Source.Length && Source[_index + 1] != '.'))
        {
            is_float = true;
            _index++;
            while (CheckBounds && (char.IsAsciiDigit(Source[_index]) || Source[_index] == '_')) _index++;
        }
        if (CheckBounds && (Source[_index] == 'e' || Source[_index] == 'E'))
        {
            is_float = true;
            _index++;
            if (CheckBounds && (Source[_index] == '+' || Source[_index] == '-')) _index++;
            var exp_digits = 0;
            while (CheckBounds && (char.IsAsciiDigit(Source[_index]) || Source[_index] == '_'))
            {
                if (char.IsAsciiDigit(Source[_index])) exp_digits++;
                _index++;
            }
            if (exp_digits == 0) throw new Exception("invalid float exponent");
        }
        if (CheckBounds && Source[_index] == 'f')
        {
            _index++;
            is_float = true;
        }
        else if (CheckBounds && char.IsAsciiLetter(Source[_index])) throw new Exception("invalid numeric literal suffix");
        Tokens.Add(new(is_float ? Tag.FloatLiteral : Tag.IntegerLiteral, start, _index));
    }

    private bool CheckBounds => _index < Source.Length;

    private void Identifier(int start)
    {
        while (CheckBounds &&
                (Source[_index] == '_'
                || char.IsAsciiLetterOrDigit(Source[_index])
             )) _index++;
        Tokens.Add(new(Token.KeywordOrIdentifier(Source[start.._index]), start, _index));
    }

    private void SkipCommentsAndWhiteSpace()
    {
        while (CheckBounds)
        {
            var el = Source[_index];
            if (char.IsWhiteSpace(el)) _index += 1;
            else if (el == '/')
            {
                if (_index + 1 >= Source.Length) return;
                if (Source[_index + 1] == '/')
                {
                    _index += 2;
                    while (CheckBounds && Source[_index] != '\n') _index += 1;
                }
                else return;
            }
            else return;
        }
    }
}
