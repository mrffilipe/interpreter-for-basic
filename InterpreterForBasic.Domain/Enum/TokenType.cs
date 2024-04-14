namespace InterpreterForBasic.Domain;

public enum TokenType
{
    Label,
    Keyword,
    Identifier,
    Operator,
    NumericLiteral,
    StringLiteral,
    Separator,
    Comment,
    EOL  // End of line
}