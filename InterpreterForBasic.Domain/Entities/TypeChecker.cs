namespace InterpreterForBasic.Domain;

public class TypeChecker
{
    public void Check(AstNode node)
    {
        switch (node)
        {
            case NumberLiteral _:
                break;
            case BinaryExpression expr:
                Check(expr.Left);
                Check(expr.Right);
                break;
            default:
                throw new Exception("Unsupported node type");
        }
    }
}