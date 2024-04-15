using InterpreterForBasic.Domain;

public class Parser
{
    private readonly List<Token> tokens;
    private int currentTokenIndex;
    private Token CurrentToken => tokens[currentTokenIndex];
    private Dictionary<string, int> variables = new Dictionary<string, int>();

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public void Parse()
    {
        while (currentTokenIndex < tokens.Count && CurrentToken.Type != TokenType.EOL)
        {
            ParseLine();
        }
    }
    private void ParseLine()
    {
        while (currentTokenIndex < tokens.Count && CurrentToken.Type != TokenType.EOL)
        {
            if (CurrentToken.Type == TokenType.Comment)
            {
                currentTokenIndex++;
                continue;
            }

            if (CurrentToken.Type == TokenType.Separator && CurrentToken.Value == ":")
            {
                currentTokenIndex++;
                continue;
            }

            switch (CurrentToken.Type)
            {
                case TokenType.Keyword when CurrentToken.Value == "IF":
                    ExecuteConditional();
                    break;
                case TokenType.Keyword when CurrentToken.Value == "PRINT":
                    ExecutePrint();
                    break;
                case TokenType.Keyword when CurrentToken.Value == "GOTO":
                    ExecuteGoto();
                    break;
                case TokenType.Keyword when CurrentToken.Value == "INPUT":
                    ExecuteInput();
                    break;
                case TokenType.Keyword when CurrentToken.Value == "HALT":
                    ExecuteHalt();
                    return; // Exit after halt
                case TokenType.Identifier:
                    ExecuteAssignment();
                    break;
                default:
                    throw new Exception($"Unexpected token: {CurrentToken.Value}");
            }
        }
        currentTokenIndex++;  // Move past the EOL
    }

    private void ExecuteHalt()
    {
        Console.WriteLine("Execution halted.");
        Environment.Exit(0);
    }

    private void ExecuteInput()
    {
        currentTokenIndex++;  // Skip 'INPUT'
        string variableName = CurrentToken.Value;
        currentTokenIndex++;  // Skip variable name
        Console.Write($"{variableName}: ");
        int value = int.Parse(Console.ReadLine());
        variables[variableName] = value;
    }

    private void ExecuteConditional()
    {
        currentTokenIndex++;  // Skip 'IF'
        bool condition = EvaluateCondition();
        if (condition)
        {
            ParseLine();  // Execute the next line if condition is true
        }
        else
        {
            // Skip to next line or handle else (not implemented here)
        }
    }

    private void ExecuteAssignment()
    {
        string variableName = CurrentToken.Value;
        currentTokenIndex++;  // Skip variable name
        currentTokenIndex++;  // Skip '='
        int value = EvaluateExpression();
        variables[variableName] = value;
    }

    private void ExecutePrint()
    {
        currentTokenIndex++;  // Skip 'PRINT'

        if (CurrentToken.Type == TokenType.StringLiteral)
        {
            // Diretamente imprime o literal string
            Console.WriteLine(CurrentToken.Value.Trim('"'));  // Remove aspas duplas antes de imprimir
            currentTokenIndex++;  // Move past the string literal
        }
        else
        {
            // Assume que o token é uma expressão numérica e avalia
            int value = EvaluateExpression();
            Console.WriteLine(value);
        }
    }

    private void ExecuteGoto()
    {
        // GOTO implementation would require more complex control flow management
        throw new NotImplementedException("GOTO not implemented.");
    }

    private int EvaluateExpression()
    {
        int leftValue;

        if (CurrentToken.Type == TokenType.Identifier)
        {
            // Handle variables
            string varName = CurrentToken.Value;
            currentTokenIndex++;  // Move to the next token (possibly an operator)

            if (!variables.TryGetValue(varName, out leftValue))
                throw new Exception($"Undefined variable {varName}");

            if (currentTokenIndex < tokens.Count && tokens[currentTokenIndex].Type == TokenType.Operator)
            {
                // There's an operator, so perform a binary operation
                return EvaluateBinaryOperation(leftValue);
            }
            else
            {
                // If the next token is not an operator, it could be EOL or ':', just return the variable's value
                // currentTokenIndex should not increment here because it might skip necessary tokens like EOL or ':'
                return leftValue;
            }
        }
        else if (CurrentToken.Type == TokenType.NumericLiteral)
        {
            // Handle numeric literals
            leftValue = int.Parse(CurrentToken.Value);
            currentTokenIndex++;  // Move past the number

            if (currentTokenIndex < tokens.Count && tokens[currentTokenIndex].Type == TokenType.Operator)
            {
                // There's an operator, so perform a binary operation
                return EvaluateBinaryOperation(leftValue);
            }
            else
            {
                // No operator, just return the number
                // Similar to the above, do not increment currentTokenIndex here
                return leftValue;
            }
        }
        else
        {
            throw new Exception("Expression format error");
        }
    }

    private int EvaluateBinaryOperation(int leftValue)
    {
        string op = tokens[currentTokenIndex].Value;
        currentTokenIndex++;  // Skip the operator
        if (currentTokenIndex >= tokens.Count)
            throw new Exception("Incomplete expression");

        int rightValue = EvaluateExpression();  // Recursively evaluate the right-hand expression

        switch (op)
        {
            case "+": return leftValue + rightValue;
            case "-": return leftValue - rightValue;
            case "*": return leftValue * rightValue;
            case "/":
                if (rightValue == 0)
                    throw new Exception("Division by zero");
                return leftValue / rightValue;
            default:
                throw new Exception($"Unsupported operator {op}");
        }
    }

    private bool EvaluateCondition()
    {
        // Similar to EvaluateExpression, but returns a boolean
        return true;  // Placeholder
    }
}