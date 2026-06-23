namespace Compiler;

using System.IO;
internal sealed class Compiler
{
    public void CompileFile(string path)
    {
        var file = File.ReadAllText(path);
        //tokenize
        var lexer = new Lexer() { Source = file };
        var tokens = lexer.Tokenize();

        //parse
        var parser = new Parser(file, tokens);
        var astRoot = parser.Parse();
        // astRoot.Print();

        //codegen
        var codegen = new LLVMCodegen(astRoot, file);
        codegen.Gencode();
    }
}
