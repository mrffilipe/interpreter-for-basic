using System.Text.RegularExpressions;

namespace InterpreterForBasic.Domain;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _position;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public AstNode Parse()
    {
        return ParseExpression();
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

        throw new Exception("Unexpected token");
    }

    private AstNode ParseStatement()
    {
        if (Match("PRINT"))
        {
            var expression = ParseExpression();

            return new PrintStatement(expression);
        }

        throw new Exception("Unknown statement");
    }

    private bool Match(string type)
    {
        if (Peek().Type == type)
        {
            Advance();

            return true;
        }

        return false;
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