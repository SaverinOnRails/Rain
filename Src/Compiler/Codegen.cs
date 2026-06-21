namespace Compiler;

internal sealed class LLVMCodegen
{
    private AstRootNode _rootNode;
    private string _source;

    public LLVMCodegen(AstRootNode rootNode, string source)
    {
        _rootNode = rootNode;
        _source = source;
    }
}
