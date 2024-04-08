namespace InterpreterForBasic.Domain;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _position;
    private Dictionary<string, AstNode> _labelMap = new Dictionary<string, AstNode>();

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public AstNode Parse()
    {
        List<AstNode> programNodes = new List<AstNode>();

        while (!AtEnd())
        {
            string label = null;
            Token currentToken = Peek();

            if (currentToken.Type != "LABEL")
            {
                label = currentToken.Value;

                Advance();
            }

            AstNode statement = ParseStatement();

            if (statement != null)
            {
                if (label != null)
                {
                    _labelMap[label] = statement;
                }

                programNodes.Add(statement);
            }
        }

        return new ProgramNode(programNodes);
    }

    private AstNode ParseExpression()
    {
        AstNode left = ParsePrimary();

        while (true)
        {
            Token operatorToken = Peek();

            if (operatorToken.Type != "PLUS" && operatorToken.Type != "MINUS")
                break;

            Advance();

            AstNode right = ParsePrimary();

            left = new BinaryExpression(left, operatorToken, right);
        }

        return left;
    }

    private AstNode ParsePrimary()
    {
        Token nextToken = Peek();

        if (nextToken.Type == "NUMBER")
        {
            Advance();
            return new NumberLiteral(nextToken.Value);
        }
        else if (nextToken.Type == "STRING")
        {
            Advance();
            return new StringLiteral(nextToken.Value);
        }

        throw new Exception($"Unexpected token: {nextToken.Type}");
    }

    private AstNode ParseStatement()
    {
        if (Match("PRINT"))
        {
            var expression = ParseExpression();

            return new PrintStatement(expression);
        }
        else
        {
            Advance();

            return null;
        }

        //throw new Exception("Unknown statement");
        return null;
    }

    private bool Match(string type)
    {
        if (!AtEnd() && Peek().Type == type)
        {
            Advance();

            return true;
        }

        return false;
    }

    private bool AtEnd()
    {
        return Peek().Type == "EOF";
    }

    private Token Peek()
    {
        if (_position < _tokens.Count)
        {
            return _tokens[_position];
        }
        else
        {
            return new Token("EOF", "");
        }
    }

    private void Advance()
    {
        if (_position < _tokens.Count)
        {
            _position++;
        }
    }
}