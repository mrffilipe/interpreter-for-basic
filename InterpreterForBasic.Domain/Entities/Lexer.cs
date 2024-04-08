namespace InterpreterForBasic.Domain;

public class Lexer
{
    private readonly string _input;
    private int _position;
    private bool _isStartOfLine = true;

    public Lexer(string input)
    {
        _input = input;
        _position = 0;
    }

    public List<Token> Tokenize()
    {
        List<Token> tokens = new List<Token>();

        while (!AtEnd())
        {
            char currentChar = Peek();

            if (char.IsWhiteSpace(currentChar))
            {
                if (currentChar == '\n')
                {
                    _isStartOfLine = true;
                }

                Advance();
            }
            else if (_isStartOfLine && char.IsDigit(currentChar))
            {
                string label = ReadNumber();
                tokens.Add(new Token("LABEL", label));
                _isStartOfLine = false;
            }
            else if (char.IsDigit(currentChar))
            {
                string number = ReadNumber();
                tokens.Add(new Token("NUMBER", number));
                _isStartOfLine = false;
            }
            else if (char.IsLetter(currentChar))
            {
                string word = ReadWord();

                if (!string.IsNullOrEmpty(word))
                {
                    string type = GetTokenType(word);
                    tokens.Add(new Token(type, word));
                }

                _isStartOfLine = false;
            }
            else if (currentChar == '\"')
            {
                string str = ReadString();
                tokens.Add(new Token("STRING", str));
                _isStartOfLine = false;
            }
            else
            {
                throw new Exception($"Caractere inesperado: {currentChar}");
            }
        }

        return tokens;
    }

    private string ReadWord()
    {
        int startPosition = _position;

        while (!AtEnd() && (char.IsLetterOrDigit(Peek()) || Peek().Equals("_")))
        {
            Advance();
        }

        string word = _input.Substring(startPosition, _position - startPosition);

        if (word.ToUpper() == "REM")
        {
            IgnoreRestOfTheLine();

            return "";
        }

        return word;
    }

    private void IgnoreRestOfTheLine()
    {
        while (!AtEnd() && Peek() != '\n')
        {
            Advance();
        }
    }

    private string ReadString()
    {
        Advance();

        int startPosition = _position;

        while (!AtEnd() && Peek() != '\"')
        {
            Advance();
        }

        if (AtEnd())
        {
            throw new Exception("String literal não fechada.");
        }

        string value = _input.Substring(startPosition, _position - startPosition);

        Advance();

        return value;
    }

    private string GetTokenType(string word)
    {
        switch (word.ToUpper())
        {
            case "PRINT":
                return "PRINT";
            default:
                return "IDENTIFIER";
        }
    }

    private bool AtEnd()
    {
        return _position >= _input.Length;
    }

    private char Peek()
    {
        return _input[_position];
    }

    private void Advance()
    {
        _position++;
    }

    private string ReadNumber()
    {
        int startPosition = _position;

        while (!AtEnd() && char.IsDigit(Peek()))
        {
            Advance();
        }

        return _input.Substring(startPosition, _position - startPosition);
    }
}