namespace InterpreterForBasic.Domain;

public class Interpreter
{
    public object Interpret(AstNode root)
    {
        return Evaluate(root);
    }

    private object Evaluate(AstNode node)
    {
        if (node == null)
        {
            return null; // para instruções não reconhecidas
        }

        switch (node)
        {
            case BinaryExpression binaryExpr:
                return EvaluateBinaryExpression(binaryExpr);
            case NumberLiteral number:
                return int.Parse(number.Value);
            case StringLiteral strLiteral:
                return strLiteral.Value;
            case PrintStatement printStatement:
                object value = Evaluate(printStatement.Expression);
                Console.WriteLine(value);
                return value;
            default:
                throw new NotImplementedException("Unknown AST Node type.");
        }
    }

    private object EvaluateBinaryExpression(BinaryExpression expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Token.Type)
        {
            case "PLUS":
                return (int)left + (int)right;
            default:
                throw new NotImplementedException("Unsupported operator.");
        }
    }
}