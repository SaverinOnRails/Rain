namespace Compiler;

internal interface IAstNode
{
    public Token FirstToken { get; set; }
    void Print();
}


internal class ProcDefAstNode : IAstNode
{
    public required ProcDeclAstNode Declaration { get; set; }
    public required IAstNode? Body { get; set; }
    public required Token FirstToken { get; set; }

    public void Print()
    {
        Declaration.Print();
        Body?.Print();
    }
}

internal class BlockAstNode : IAstNode
{
    public List<IAstNode> Statements = new();
    public required Token FirstToken { get; set; }

    public void Print()
    {
        foreach (var s in Statements)
        {
            s.Print();
        }
    }
}

internal class VariableDeclStatementAstNode : IAstNode
{
    public required string Name { get; set; }
    public IAstNode? Expression = null;
    public required Token FirstToken { get; set; }
    public void Print()
    {
        Console.WriteLine($"Assigment statement with name {Name}");
        Expression?.Print();
    }
}

internal class ExpressionAstNode : IAstNode
{
    public required string Number { get; set; }
    public required Token FirstToken { get; set; }
    public void Print()
    {
        Console.WriteLine($"Expression ast with value {Number}");
    }
}

internal class ProcDeclAstNode : IAstNode
{
    public required string Name;
    public required Token FirstToken { get; set; }
    public List<IAstNode> parameters = new(); //for now
    public required IAstNode? returnType { get; set; }

    public void Print()
    {
        Console.WriteLine($"PROC_DEF_NODE with {Name} and param list {parameters.Count()} and return {returnType}");
    }}

internal sealed class AstRootNode : IAstNode
{
    public List<IAstNode> TopLevelDecls { get; set; } = new();
    public required Token FirstToken { get; set; }

    public void Print()
    {
        foreach (var toplevel in TopLevelDecls)
        {
            toplevel.Print();
        }
    }
}


