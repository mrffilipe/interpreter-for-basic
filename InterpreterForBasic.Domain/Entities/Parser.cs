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

        // Certifique-se de não processar uma linha além do EOL
        while (currentTokenIndex < tokens.Count && CurrentToken.Type != TokenType.EOL)
        {
            if (CurrentToken.Type == TokenType.Comment)
            {
                // Pula o token de comentário
                currentTokenIndex++;
                // Pula qualquer token de EOL após um comentário
                while (currentTokenIndex < tokens.Count && CurrentToken.Type == TokenType.EOL)
                {
                    currentTokenIndex++;
                }
                continue;  // Continua para o próximo token
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
                    // Após um HALT não deve haver mais tokens a serem processados nesta linha
                    currentTokenIndex = tokens.Count;
                    break;
                case TokenType.Identifier:
                    node = ParseAssignment();
                    break;
                default:
                    throw new Exception($"Unexpected token: {CurrentToken.Value}");
            }

            // Se um nó foi criado, significa que a linha foi processada.
            // Se o nó não for um comando de fluxo de controle (como IF ou GOTO), saia do loop.
            if (node != null && !(node is ConditionalNode) && !(node is GotoNode))
            {
                break;
            }
        }

        // Pula o token EOL apenas se ainda houver tokens restantes para processar.
        if (currentTokenIndex < tokens.Count && CurrentToken.Type == TokenType.EOL)
        {
            currentTokenIndex++;
        }
        return node;
    }

    private HaltNode ParseHalt()
    {
        // Já sabemos que o token atual é 'HALT', então avançamos para o próximo token.
        currentTokenIndex++;

        // Se o 'HALT' for o último token, não precisamos fazer mais nada.
        if (currentTokenIndex >= tokens.Count || tokens[currentTokenIndex].Type == TokenType.EOL)
        {
            return new HaltNode();
        }

        // Se houver mais tokens após 'HALT', isso pode ser um erro de sintaxe, 
        // porque 'HALT' deve ser o último comando executável em uma linha/programa.
        // Aqui você pode decidir se quer lançar um erro ou apenas registrar um aviso.
        throw new Exception("Unexpected tokens after 'HALT'");
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