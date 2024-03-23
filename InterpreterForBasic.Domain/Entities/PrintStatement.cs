namespace InterpreterForBasic.Domain;

public class PrintStatement : AstNode
{
    public AstNode Expression { get; private set; }

    public PrintStatement(AstNode expression)
    {
        Expression = expression;
    }
}