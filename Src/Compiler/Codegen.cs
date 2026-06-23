namespace Compiler;

using LLVMSharp.Interop;

internal sealed class LLVMCodegen
{
    private AstRootNode _rootNode;
    private string _source;
    private LLVMModuleRef _moduleRef;
    List<string> functionDefs = new();


    public LLVMCodegen(AstRootNode rootNode, string source)
    {
        _rootNode = rootNode;
        _source = source;
        _moduleRef = LLVMModuleRef.CreateWithName("rain");
    }

    public void Gencode()
    {
        try
        {
            AnalyzeNode(_rootNode);
        }
        catch (DiagnosticsException e)
        {
            DiagnosticsPrinter.PrintDiagnostic(e.Message, e.Token);
        }
    }

    private void AnalyzeNode(IAstNode node)
    {
        switch (node)
        {
            case AstRootNode root:
                foreach (var topLevel in root.TopLevelDecls)
                {
                    AnalyzeNode(topLevel);
                }
                break;

            case ProcDefAstNode procDef:
                AnalyzeNode(procDef.Declaration);
                if (procDef.Body is not null)
                {
                    AnalyzeNode(procDef.Body);
                }
                break;
            case ProcDeclAstNode procDecl:
                var name = procDecl.Name;
                if (functionDefs.Contains(name))
                {
                    throw new DiagnosticsException($"Redefinition of procedure {name}", node.FirstToken);
                }
                functionDefs.Add(name);
                break;
        }
    }
}
