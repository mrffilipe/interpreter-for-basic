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
        FlattenTokens();
    }

    private void FlattenTokens()
    {
        tokens = new List<Token>();

        foreach (var line in programLines)
        {
            tokens.AddRange(line.Value);

            if (line.Value.Count > 0 && line.Value.Last().Type != TokenType.EOL)
                tokens.Add(new Token(TokenType.EOL, "\n"));
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
                    return;
                case TokenType.Identifier:
                    ExecuteAssignment();
                    break;
                default:
                    throw new Exception($"Unexpected token: {CurrentToken.Value}");
            }
        }

        currentTokenIndex++;
    }

    private void ExecuteGoto()
    {
        currentTokenIndex++;

        if (CurrentToken.Type != TokenType.NumericLiteral)
            throw new Exception("Syntax error in GOTO statement: Line number expected.");

        int lineNumber = int.Parse(CurrentToken.Value);
        if (!programLines.ContainsKey(lineNumber))
            throw new Exception($"Line number {lineNumber} not found in program lines.");

        currentTokenIndex = FindTokenIndexByLineNumber(lineNumber);
    }

    private int FindTokenIndexByLineNumber(int lineNumber)
    {
        int index = 0;

        foreach (var line in programLines)
        {
            if (line.Key == lineNumber)
            {
                return index;
            }

            index += line.Value.Count;
        }

        throw new Exception("Line number not found in tokens.");
    }

    private void ExecuteHalt()
    {
        Console.WriteLine("Execution halted.");
        Environment.Exit(0);
    }

    private void ExecuteInput()
    {
        currentTokenIndex++;
        string variableName = CurrentToken.Value;
        currentTokenIndex++;
        Console.Write($"{variableName}: ");
        int value = int.Parse(Console.ReadLine());
        variables[variableName] = value;
    }

    private void ExecuteConditional()
    {
        currentTokenIndex++;
        bool condition = EvaluateCondition();

        if (condition)
        {
            if (CurrentToken.Type == TokenType.Keyword && CurrentToken.Value.ToUpper() == "GOTO")
            {
                ExecuteGoto();
            }
            else
            {
                ParseLine();
            }
        }
        else
        {
            SkipToNextLine();
        }
    }

    private void SkipToNextLine()
    {
        while (currentTokenIndex < tokens.Count && CurrentToken.Type != TokenType.EOL)
        {
            currentTokenIndex++;
        }

        currentTokenIndex++;
    }

    private void ExecuteAssignment()
    {
        string variableName = CurrentToken.Value;
        currentTokenIndex++;
        currentTokenIndex++;
        int value = EvaluateExpression();
        variables[variableName] = value;
    }

    private void ExecutePrint()
    {
        currentTokenIndex++;

        if (CurrentToken.Type == TokenType.StringLiteral)
        {
            Console.WriteLine(CurrentToken.Value.Trim('"'));
            currentTokenIndex++;
        }
        else
        {
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
            currentTokenIndex++;

            if (!variables.TryGetValue(varName, out leftValue))
                throw new Exception($"Undefined variable {varName}");

            if (currentTokenIndex < tokens.Count && IsArithmeticOperator(tokens[currentTokenIndex].Value))
            {
                return EvaluateBinaryOperation(leftValue);
            }
            else
            {
                return leftValue;
            }
        }
        else if (CurrentToken.Type == TokenType.NumericLiteral)
        {
            leftValue = int.Parse(CurrentToken.Value);
            currentTokenIndex++;

            if (currentTokenIndex < tokens.Count && IsArithmeticOperator(tokens[currentTokenIndex].Value))
            {
                return EvaluateBinaryOperation(leftValue);
            }
            else
            {
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
        currentTokenIndex++;

        if (!IsArithmeticOperator(op))
            throw new Exception("Attempted to use a non-arithmetic operator in an arithmetic context");

        int rightValue = EvaluateExpression();

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

    private bool EvaluateCondition()
    {
        int leftValue = EvaluateExpression();

        if (currentTokenIndex >= tokens.Count || !IsComparisonOperator(tokens[currentTokenIndex].Value))
            throw new Exception("Expected comparison operator");

        string operator1 = tokens[currentTokenIndex].Value;
        currentTokenIndex++;

        int rightValue = EvaluateExpression();

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