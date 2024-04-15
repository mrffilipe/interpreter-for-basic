using InterpreterForBasic.Domain;

public class Parser
{
    private List<Token> tokens;
    private int currentTokenIndex;
    private Token CurrentToken => tokens[currentTokenIndex];
    private Dictionary<string, int> variables = new Dictionary<string, int>();
    private Dictionary<int, List<Token>> programLines;

    public Parser(Dictionary<int, List<Token>> programLines)
    {
        this.programLines = programLines;
        FlattenTokens();  // Initialize tokens list from program lines
    }

    private void FlattenTokens()
    {
        tokens = new List<Token>();
        foreach (var line in programLines)
        {
            tokens.AddRange(line.Value);  // Add all tokens from each line
            if (line.Value.Count > 0 && line.Value.Last().Type != TokenType.EOL)
                tokens.Add(new Token(TokenType.EOL, "\n"));  // Ensure each line ends with EOL
        }
    }

    public void Parse()
    {
        while (currentTokenIndex < tokens.Count)
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
                currentTokenIndex++;  // Skip comments
                continue;
            }

            if (CurrentToken.Type == TokenType.Separator && CurrentToken.Value == ":")
            {
                currentTokenIndex++;  // Skip separators
                continue;
            }

            switch (CurrentToken.Type)
            {
                case TokenType.Keyword when CurrentToken.Value.ToUpper() == "IF":
                    ExecuteConditional();
                    break;
                case TokenType.Keyword when CurrentToken.Value.ToUpper() == "PRINT":
                    ExecutePrint();
                    break;
                case TokenType.Keyword when CurrentToken.Value.ToUpper() == "GOTO":
                    ExecuteGoto();
                    break;
                case TokenType.Keyword when CurrentToken.Value.ToUpper() == "INPUT":
                    ExecuteInput();
                    break;
                case TokenType.Keyword when CurrentToken.Value.ToUpper() == "HALT":
                    ExecuteHalt();
                    return;  // Halt execution
                case TokenType.Identifier:
                    ExecuteAssignment();
                    break;
                default:
                    throw new Exception($"Unexpected token: {CurrentToken.Value}");
            }
        }
        currentTokenIndex++;  // Move past the EOL
    }

    private void ExecuteGoto()
    {
        currentTokenIndex++;  // Skip 'GOTO'
        if (CurrentToken.Type != TokenType.NumericLiteral)
            throw new Exception("Syntax error in GOTO statement: Line number expected.");

        int lineNumber = int.Parse(CurrentToken.Value);
        if (!programLines.ContainsKey(lineNumber))
            throw new Exception($"Line number {lineNumber} not found in program lines.");

        // Find the index of the first token of the target line in the flat token list
        currentTokenIndex = FindTokenIndexByLineNumber(lineNumber);  // Adjust currentTokenIndex to the start of the target line
    }

    private int FindTokenIndexByLineNumber(int lineNumber)
    {
        int index = 0;
        foreach (var line in programLines)
        {
            if (line.Key == lineNumber)
            {
                return index;  // Return the index of the first token on the target line
            }
            index += line.Value.Count;  // Add the number of tokens in the current line to index
        }
        throw new Exception("Line number not found in tokens.");  // Line number not found
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
            if (CurrentToken.Type == TokenType.Keyword && CurrentToken.Value.ToUpper() == "GOTO")
            {
                ExecuteGoto();  // Perform the GOTO if the condition is true
            }
            else
            {
                ParseLine();  // Execute the next line if condition is true and no GOTO
            }
        }
        else
        {
            // Skip to next line
            SkipToNextLine();
        }
    }

    private void SkipToNextLine()
    {
        while (currentTokenIndex < tokens.Count && CurrentToken.Type != TokenType.EOL)
        {
            currentTokenIndex++;
        }
        currentTokenIndex++;  // Skip the EOL token
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

    private int EvaluateExpression()
    {
        int leftValue;

        if (CurrentToken.Type == TokenType.Identifier)
        {
            string varName = CurrentToken.Value;
            currentTokenIndex++;  // Move to the next token (possibly an operator)

            if (!variables.TryGetValue(varName, out leftValue))
                throw new Exception($"Undefined variable {varName}");

            if (currentTokenIndex < tokens.Count && IsArithmeticOperator(tokens[currentTokenIndex].Value))
            {
                return EvaluateBinaryOperation(leftValue);
            }
            else
            {
                return leftValue;  // No arithmetic operator, return the variable's value
            }
        }
        else if (CurrentToken.Type == TokenType.NumericLiteral)
        {
            leftValue = int.Parse(CurrentToken.Value);
            currentTokenIndex++;  // Move past the number

            if (currentTokenIndex < tokens.Count && IsArithmeticOperator(tokens[currentTokenIndex].Value))
            {
                return EvaluateBinaryOperation(leftValue);
            }
            else
            {
                return leftValue;  // No arithmetic operator, just return the number
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

        if (!IsArithmeticOperator(op))
            throw new Exception("Attempted to use a non-arithmetic operator in an arithmetic context");

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
                throw new Exception($"Unsupported arithmetic operator {op}");
        }
    }

    private bool IsArithmeticOperator(string op)
    {
        return op == "+" || op == "-" || op == "*" || op == "/";
    }

    private bool IsComparisonOperator(string op)
    {
        return op == ">" || op == "<" || op == "==" || op == "!=" || op == ">=" || op == "<=";
    }

    private int PerformOperation(int left, int right, string op)
    {
        switch (op)
        {
            case "+": return left + right;
            case "-": return left - right;
            case "*": return left * right;
            case "/":
                if (right == 0)
                    throw new Exception("Division by zero");
                return left / right;
            default:
                throw new Exception($"Unsupported arithmetic operator {op}");
        }
    }

    private bool EvaluateCondition()
    {
        int leftValue = EvaluateExpression();  // Evaluate the left side of the condition

        if (currentTokenIndex >= tokens.Count || !IsComparisonOperator(tokens[currentTokenIndex].Value))
            throw new Exception("Expected comparison operator");

        string operator1 = tokens[currentTokenIndex].Value;
        currentTokenIndex++;  // Move past the operator

        int rightValue = EvaluateExpression();  // Evaluate the right side of the condition

        bool result = Compare(leftValue, rightValue, operator1);

        return result;
    }



    private bool Compare(int left, int right, string op)
    {
        switch (op)
        {
            case ">": return left > right;
            case "<": return left < right;
            case "==": return left == right;
            case "!=": return left != right;
            case ">=": return left >= right;
            case "<=": return left <= right;
            default: throw new Exception($"Unsupported comparison operator {op}");
        }
    }
}