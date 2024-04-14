namespace InterpreterForBasic.Domain;

public class Lexer
{
    public Dictionary<int, string> ProgramLines { get; private set; }

    public Lexer()
    {
        ProgramLines = new Dictionary<int, string>();
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
                ProgramLines[lineNumber] = content;
            }
        }
    }
}