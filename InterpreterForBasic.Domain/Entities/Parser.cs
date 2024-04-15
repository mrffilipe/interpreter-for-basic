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
        int value = EvaluateExpression();
        Console.WriteLine(value);
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
                // No operator, just return the variable's value
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

        int rightValue;
        if (tokens[currentTokenIndex].Type == TokenType.Identifier)
        {
            string varName = tokens[currentTokenIndex].Value;
            if (!variables.TryGetValue(varName, out rightValue))
                throw new Exception($"Undefined variable {varName}");
        }
        else if (tokens[currentTokenIndex].Type == TokenType.NumericLiteral)
        {
            rightValue = int.Parse(tokens[currentTokenIndex].Value);
        }
        else
        {
            throw new Exception("Right-hand side of expression error");
        }

        currentTokenIndex++;  // Move past the right value

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