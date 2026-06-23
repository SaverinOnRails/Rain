namespace Compiler;

internal static class DiagnosticsPrinter
{
    internal static void PrintDiagnostic(string message, Token token)
    {
        Console.WriteLine($"{message} at Line {token.StartLine} Column {token.StartColumn}");
        System.Environment.Exit(1);
    }

    internal static void PrintDiagnostic(string message)
    {
        Console.WriteLine($"{message}");
        System.Environment.Exit(1);
    }
}

internal class DiagnosticsException : Exception
{
    public Token Token {get;}
    public DiagnosticsException(string msg, Token token) : base(msg)
    {
        Token = token;
    }
}
