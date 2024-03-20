namespace InterpreterForBasic.Domain;

public class BinaryExpression
{
    public AstNode Left { get; private set; }
    public Token Token { get; private set; }
    public AstNode Right { get; private set; }

    public BinaryExpression(AstNode left, Token op, AstNode right)
    {
        Left = left;
        Token = op;
        Right = right;
    }
}