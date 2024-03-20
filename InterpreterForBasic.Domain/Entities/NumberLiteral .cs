namespace InterpreterForBasic.Domain;

public class NumberLiteral
{
    public string Value { get; private set; }

    public NumberLiteral(string value)
    {
        Value = value;
    }
}