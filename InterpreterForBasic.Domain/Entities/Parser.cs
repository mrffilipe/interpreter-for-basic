using InterpreterForBasic.Domain;

public class Parser
{
    private readonly List<Token> tokens;
    private int currentTokenIndex;
    private Token CurrentToken => tokens[currentTokenIndex];

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public AstNode Parse()
    {
        AstNode rootNode = null;
        while (currentTokenIndex < tokens.Count && CurrentToken.Type != TokenType.EOL)
        {
            var node = ParseLine();
            if (rootNode == null)
                rootNode = node;
            // Para uma estrutura mais complexa, você pode precisar de uma forma de conectar múltiplos nós de linha
            // Por exemplo, adicionando-os a uma lista ou a um nó "root" que os contém todos
        }
        return rootNode;
    }

    private AstNode ParseLine()
    {
        AstNode node = null;

        // O loop continua até que seja o fim da linha ou não haja mais tokens.
        while (currentTokenIndex < tokens.Count && CurrentToken.Type != TokenType.EOL)
        {
            if (CurrentToken.Type == TokenType.Comment)
            {
                // Pula os tokens de comentário.
                currentTokenIndex++;
                if (currentTokenIndex < tokens.Count && CurrentToken.Type == TokenType.EOL)
                {
                    // Se o próximo token for EOL, saia do loop.
                    break;
                }
                continue;  // Continua para o próximo token.
            }

            switch (CurrentToken.Type)
            {
                case TokenType.Keyword when CurrentToken.Value == "IF":
                    node = ParseConditional();
                    break;
                case TokenType.Keyword when CurrentToken.Value == "PRINT":
                    node = ParsePrint();
                    break;
                case TokenType.Keyword when CurrentToken.Value == "GOTO":
                    node = ParseGoto();
                    break;
                case TokenType.Keyword when CurrentToken.Value == "INPUT":
                    node = ParseInput();
                    break;
                case TokenType.Keyword when CurrentToken.Value == "HALT":
                    node = ParseHalt();
                    break;
                case TokenType.Identifier:
                    node = ParseAssignment();
                    break;
                default:
                    throw new Exception($"Unexpected token: {CurrentToken.Value}");
            }

            // Se o node foi criado, o resto da linha deve ser relacionado a este comando, então devemos parar de analisar.
            if (node != null)
            {
                break;
            }
        }

        // Pula o token EOL apenas se ainda houver tokens restantes para processar.
        if (currentTokenIndex < tokens.Count)
        {
            currentTokenIndex++;  // Pula o token EOL, se presente.
        }
        return node;
    }

    private HaltNode ParseHalt()
    {
        currentTokenIndex++;  // Skip 'HALT'
        return new HaltNode();
    }

    private InputNode ParseInput()
    {
        currentTokenIndex++;  // Skip 'INPUT'
        var variable = new VariableNode { Name = CurrentToken.Value };
        currentTokenIndex++;  // Skip variable name
        return new InputNode { Variable = variable };
    }

    private ConditionalNode ParseConditional()
    {
        currentTokenIndex++;  // Skip 'IF'
        var condition = ParseExpression();
        var trueBranch = new List<AstNode>();
        while (CurrentToken.Type != TokenType.EOL)
        {
            trueBranch.Add(ParseLine());
        }
        return new ConditionalNode { Condition = condition, TrueBranch = trueBranch };
    }

    private AssignmentNode ParseAssignment()
    {
        var variable = new VariableNode { Name = CurrentToken.Value };
        currentTokenIndex++;  // Skip variable name
        currentTokenIndex++;  // Skip '='
        var expression = ParseExpression();
        return new AssignmentNode { Variable = variable, Expression = expression };
    }

    private PrintNode ParsePrint()
    {
        currentTokenIndex++;  // Skip 'PRINT'
        var expression = ParseExpression();
        return new PrintNode { Expression = expression };
    }

    private GotoNode ParseGoto()
    {
        currentTokenIndex++;  // Skip 'GOTO'
        var target = int.Parse(CurrentToken.Value);
        currentTokenIndex++;  // Skip target line number
        return new GotoNode { Target = target };
    }

    private AstNode ParseExpression()
    {
        AstNode left = CurrentToken.Type == TokenType.Identifier ? (AstNode)new VariableNode { Name = CurrentToken.Value } : new ConstantNode { Value = CurrentToken.Value };
        currentTokenIndex++;
        if (currentTokenIndex < tokens.Count && tokens[currentTokenIndex].Type == TokenType.Operator)
        {
            var op = CurrentToken.Value;
            currentTokenIndex++;
            var right = ParseExpression();
            return new BinaryOperationNode { Left = left, Operator = op, Right = right };
        }
        return left;
    }
}