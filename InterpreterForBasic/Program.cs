using InterpreterForBasic.Domain;

namespace InterpreterForBasic;

internal class Program
{
    static void Main(string[] args)
    {

        Console.WriteLine("Basic Interpreter");
        Console.WriteLine("Enter BASIC code, or type 'exit' to quit:");

        while (true)
        {
            Console.WriteLine(">>");

            string input = Console.ReadLine();

            if (input.ToLower() == "exit")
            {
                break;
            }

            try
            {
                Lexer lexer = new Lexer(input);
                List<Token> tokens = lexer.Tokenize();

                Parser parser = new Parser(tokens);
                AstNode ast = parser.Parse();

                Interpreter interpreter = new Interpreter();
                interpreter.Interpret(ast);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}