using InterpreterForBasic.Domain;

namespace InterpreterForBasic;

internal class Program
{
    static void Main(string[] args)
    {
        string filePath = "assets/file/example.basic";

        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                Lexer lexer = new Lexer();
                lexer.Tokenize(lines);  // Tokeniza todas as linhas e armazena em ProgramLines

                //List<Token> allTokens = new List<Token>();  // Lista para armazenar todos os tokens de todas as linhas
                //foreach (var lineTokens in lexer.ProgramLines.Values)  // Itera sobre cada lista de tokens em cada linha
                //{
                //    allTokens.AddRange(lineTokens);  // Adiciona os tokens da linha atual à lista total de tokens
                //}

                Parser parser = new Parser(lexer.ProgramLines);  // Cria uma instância do Parser com a lista total de tokens
                parser.Parse();
                // O próximo passo seria usar um visitor para percorrer a AST, como mostrado anteriormente
                // PrintVisitor visitor = new PrintVisitor();
                // ast.Accept(visitor);

                // Por agora, vamos assumir que você só quer verificar que a AST foi construída
                Console.WriteLine("AST construída com sucesso.");
            }
            else
            {
                Console.WriteLine($"Error: File does not exist at path {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}