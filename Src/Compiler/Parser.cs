namespace Compiler;

internal sealed class Parser
{
    public string Source { get; private set; }
    public List<Token> Tokens { get; private set; }
    private int _index = 0;

    public Parser(string source, List<Token> tokens)
    {
        Source = source;
        Tokens = tokens;
    }

    public AstRootNode Parse()
    {
        var root = new AstRootNode() { FirstToken = Tokens.First() };
        while (!Check(Tag.EOF))
        {
            try
            {
                root.TopLevelDecls.Add(ParseToplevelDecl());
            }
            catch (DiagnosticsException e)
            {
                DiagnosticsPrinter.PrintDiagnostic(e.Message, e.Token);
            }
        }
        return root;
    }
    IAstNode ParseToplevelDecl()
    {
        if (Check(Tag.KeywordVoid) && Peek(1) == Tag.Identifier && Peek(2) == Tag.LeftBracket)
        {
            return ParseProcedureDef();
        }
        var cur = Tokens[_index];
        throw new DiagnosticsException("Expected top level declaration here", cur);
    }

    private IAstNode ParseProcedureDef()
    {
        var proc_return_type_token = Expect(Tag.KeywordVoid, "Expected procedure return type here.");
        string proc_return_type = Source[proc_return_type_token.Start..proc_return_type_token.End];
        var ident_token = Expect(Tag.Identifier, "Expected procedure identifier here");
        var ident = Source[ident_token.Start..ident_token.End];
        Expect(Tag.LeftBracket, "Expected left bracket here");
        Expect(Tag.RightBracket, "Expected closing bracket here");

        var fnProto = new ProcDeclAstNode()
        {
            Name = ident,
            returnType = null,
            FirstToken = proc_return_type_token
        };
        var body = ParseBlock();
        return new ProcDefAstNode()
        {
            Declaration = fnProto,
            Body = body,
            FirstToken = proc_return_type_token
        };
    }

    private IAstNode? ParseBlock()
    {
        var lb = Expect(Tag.LeftBrace, "Expected '{' here");
        BlockAstNode node = new() { FirstToken = lb };
        while (true)
        {
            if (Match(Tag.RightBrace))
            {
                return node;
            }
            else
            {
                node.Statements.Add(ParseStatement());
            }

        }
    }

    private IAstNode ParseStatement()
    {
        if (Match(Tag.KeywordVar))
        {
            var nameToken = Expect(Tag.Identifier, "Expected an identifier here");
            var name = Source[nameToken.Start..nameToken.End];
            VariableDeclStatementAstNode assignmentNode = new() { Name = name, FirstToken = nameToken };
            if (Match(Tag.Semicolon)) return assignmentNode;
            Expect(Tag.Equal, "Expected an '=' here");
            var curr = Tokens[_index];
            if (Check(Tag.IntegerLiteral) ||
                Check(Tag.FloatLiteral) ||
                Check(Tag.Identifier))
            {
                var expressionNode = ParseExpression();
                assignmentNode.Expression = expressionNode;
                return assignmentNode;
            }
            else
            {
                throw new DiagnosticsException("Expected number assignment here", curr);
            }
        }
        else
        {
            throw new DiagnosticsException("Expected valid statement here", Tokens[_index]);
        }
    }

    //simple numeric assignment for now
    private IAstNode ParseExpression()
    {
        if (Check(Tag.IntegerLiteral) || Check(Tag.FloatLiteral))
        {
            var numToken = Tokens[_index];
            var num = Source[numToken.Start..numToken.End];
            ExpressionAstNode node = new() { Number = num, FirstToken = numToken };
            Advance();
            Expect(Tag.Semicolon, "Expected a semi colon here");
            return node;
        }
        else
        {
            throw new DiagnosticsException("Expected number assignment here", Tokens[_index]);
        }
    }

    Tag Peek(int offset)
    {
        if (_index + offset >= Tokens.Count) return Tokens.Last().Tag;
        return Tokens[_index + offset].Tag;
    }

    bool Check(Tag tag)
    {
        return Tokens[_index].Tag == tag;
    }

    Token Advance()
    {
        return Tokens[_index++];
    }

    bool Match(Tag tag)
    {
        if (Check(tag))
        {
            Advance();
            return true;
        }

        return false;
    }

    Token Expect(Tag tag, string msg)
    {
        if (!Check(tag))
        {
            var curr = Tokens[_index];
            throw new DiagnosticsException(msg, curr);
        }
        return Advance();
    }
}

