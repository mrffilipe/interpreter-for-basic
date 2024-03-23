namespace InterpreterForBasic.Domain;

public class NumberLiteral : AstNode
{
    public string Value { get; private set; }

    public NumberLiteral(string value)
    {
        Value = value;
    }
}