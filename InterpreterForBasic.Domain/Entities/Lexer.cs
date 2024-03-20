namespace InterpreterForBasic.Domain;

public class Lexer
{
    private readonly string _input;
    private int _position;

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
                Advance();
            }
            else if (char.IsDigit(currentChar))
            {
                string number = ReadNumber();
                tokens.Add(new Token("NUMBER", number));
            }
            else if (char.IsLetter(currentChar))
            {
                string word = ReadWord();
                string type = GetTokenType(word);
                tokens.Add(new Token(type, word));
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

        while (!AtEnd() && (char.IsLetterOrDigit(Peek()) || Peek().Equals("-")))
        {
            Advance();
        }

        return _input.Substring(startPosition, _position - startPosition);
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