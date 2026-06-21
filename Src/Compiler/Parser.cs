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
        var root = new AstRootNode();
        while (!Check(Tag.EOF))
        {
            root.TopLevelDecls.Add(ParseToplevelDecl());
        }
        return root;
    }
    IAstNode ParseToplevelDecl()
    {
        if (Check(Tag.KeywordVoid) && Peek(1) == Tag.Identifier && Peek(2) == Tag.LeftBracket)
        {
            return ParseProcedureDef();
        }
        throw new Exception("Expected Top Level Declaration here");
    }

    private IAstNode ParseProcedureDef()
    {
        var proc_return_type_token = Expect(Tag.KeywordVoid, "Expected procedure return type here.");
        string proc_return_type = Source[proc_return_type_token.Start..proc_return_type_token.End];
        var ident_token = Expect(Tag.Identifier, "Expected procedure identifier here");
        var ident = Source[ident_token.Start..ident_token.End];
        Expect(Tag.LeftBracket, "Expected left bracket here");
        Expect(Tag.RightBracket, "Expected closing bracket here");

        //parse function body here:
        var fnProto = new ProcDeclAstNode()
        {
            Name = ident,
            returnType = null,
        };
        var body = ParseBlock();
        return new ProcDefAstNode()
        {
            FnProto = fnProto,
            Body = body
        };
    }

    private IAstNode? ParseBlock()
    {
        Expect(Tag.LeftBrace, "Expected '{' here");
        BlockAstNode node = new();
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
        throw new Exception("Invalid Token");
    }

    private IAstNode ParseStatement()
    {
        if (Match(Tag.KeywordVar))
        {
            var nameToken = Expect(Tag.Identifier, "Expected an identifier here");
            var name = Source[nameToken.Start..nameToken.End];
            AssignmentStatementAstNode assignmentNode = new() { Name = name };
            if (Match(Tag.Semicolon)) return assignmentNode;
            Expect(Tag.Equal, "Expected an '=' here");
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
                throw new Exception("Expected number assignment here");
            }
        }
        else
        {
            throw new Exception("Expected valid statement here");
        }
    }

    //simple numeric assignment for now
    private IAstNode ParseExpression()
    {
        if (Check(Tag.IntegerLiteral) || Check(Tag.FloatLiteral))
        {
            var numToken = Tokens[_index];
            var num = Source[numToken.Start..numToken.End];
            Console.WriteLine(num);
            ExpressionAstNode node = new() { Number = num };
            Advance();
            Expect(Tag.Semicolon, "Expected a semi colon here");
            return node;
        }
        else
        {
            throw new Exception("Expected number assignment here");
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
            throw new Exception(msg);

        return Advance();
    }
}
