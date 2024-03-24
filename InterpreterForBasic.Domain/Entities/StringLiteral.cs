namespace InterpreterForBasic.Domain;

public class StringLiteral : AstNode
{
    public string Value { get; private set; }

    public StringLiteral(string value)
    {
        Value = value;
    }
}