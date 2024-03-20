using InterpreterForBasic.Domain;

namespace InterpreterForBasic;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            string code = "10 PRINT 123";
            Lexer lexer = new Lexer(code);
            List<Token> tokens = lexer.Tokenize();

            foreach (var token in tokens)
            {
                Console.WriteLine($"Tipo: {token.Type}, Valor: {token.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}