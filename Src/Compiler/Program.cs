public class Program
{
    private static Compiler.Compiler _compiler = new();
    public static void Main(String[] args)
    {
        var input_file = args[0];
        _compiler.CompileFile(input_file);
    }
}
