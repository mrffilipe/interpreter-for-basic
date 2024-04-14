namespace InterpreterForBasic.Domain;

public class Lexer
{
    public Dictionary<int, List<Token>> ProgramLines { get; private set; }

    public Lexer()
    {
        ProgramLines = new Dictionary<int, List<Token>>();
    }

    public void Tokenize(string[] lines)
    {
        foreach (var line in lines)
        {
            ProcessLine(line);
        }
    }

    private void ProcessLine(string line)
    {
        int firstSpaceIndex = line.IndexOf(' ');

        if (firstSpaceIndex > 0)
        {
            string labelString = line.Substring(0, firstSpaceIndex);

            if (int.TryParse(labelString, out int lineNumber))
            {
                string content = line.Substring(firstSpaceIndex + 1).Trim();
                List<Token> tokens = TokenizeContent(content);
                ProgramLines[lineNumber] = tokens;
            }
        }
    }

    private List<Token> TokenizeContent(string content)
    {
        List<Token> tokens = new List<Token>();

        // Tratar comentários primeiro
        if (content.StartsWith("REM "))
        {
            tokens.Add(new Token(TokenType.Comment, content.Substring(4).Trim())); // Captura tudo após "REM"
            tokens.Add(new Token(TokenType.EOL, "\n"));  // Adiciona token de fim de linha
            return tokens;
        }

        int currentIndex = 0;

        while (currentIndex < content.Length)
        {
            if (char.IsWhiteSpace(content[currentIndex]))
            {
                currentIndex++;
                continue;
            }

            // Check for Numeric Literal
            if (char.IsDigit(content[currentIndex]))
            {
                string number = ExtractNumber(content, ref currentIndex);
                tokens.Add(new Token(TokenType.NumericLiteral, number));
                continue;
            }

            // Check for String Literal
            if (content[currentIndex] == '"')
            {
                string str = ExtractString(content, ref currentIndex);
                tokens.Add(new Token(TokenType.StringLiteral, str));
                continue;
            }

            // Check for Operators
            if ("+-*/=><".Contains(content[currentIndex]))
            {
                string op = ExtractOperator(content, ref currentIndex);
                tokens.Add(new Token(TokenType.Operator, op));
                continue;
            }

            // Check for Separators
            if (content[currentIndex] == ':')
            {
                tokens.Add(new Token(TokenType.Separator, ":"));
                currentIndex++;
                continue;
            }

            // Extract Identifiers or Keywords
            string word = ExtractWord(content, ref currentIndex);
            if (IsKeyword(word))
            {
                tokens.Add(new Token(TokenType.Keyword, word));
            }
            else
            {
                tokens.Add(new Token(TokenType.Identifier, word));
            }
        }

        tokens.Add(new Token(TokenType.EOL, "\n"));  // Adiciona token de fim de linha
        return tokens;
    }

    private string ExtractNumber(string content, ref int index)
    {
        int start = index;
        while (index < content.Length && char.IsDigit(content[index]))
            index++;
        return content.Substring(start, index - start);
    }

    private string ExtractString(string content, ref int index)
    {
        index++;  // skip the opening quote
        int start = index;
        while (index < content.Length && content[index] != '"')
            index++;
        string result = content.Substring(start, index - start);
        index++;  // skip the closing quote
        return result;
    }

    private string ExtractOperator(string content, ref int index)
    {
        int start = index;
        if (content[start] == '<' || content[start] == '>')
        {
            if (index + 1 < content.Length && content[index + 1] == '=')
                index++;
        }
        index++;
        return content.Substring(start, index - start);
    }

    private string ExtractWord(string content, ref int index)
    {
        int start = index;
        while (index < content.Length && char.IsLetter(content[index]))
            index++;
        return content.Substring(start, index - start);
    }

    private bool IsKeyword(string word)
    {
        return new HashSet<string> { "REM", "PRINT", "INPUT", "IF", "THEN", "GOTO", "HALT" }.Contains(word);
    }

    private TokenType DetermineType(string part)
    {
        // A simple type determination logic (to be expanded)
        if (part == "REM")
            return TokenType.Comment;
        if (char.IsDigit(part[0]))
            return TokenType.NumericLiteral;
        if (part[0] == '"' && part[part.Length - 1] == '"')
            return TokenType.StringLiteral;
        return TokenType.Identifier;  // Default to identifier
    }
}