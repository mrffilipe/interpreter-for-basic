namespace InterpreterForBasic.Domain;

public class ProgramNode : AstNode
{
    public List<AstNode> Statements { get; private set; }

    public ProgramNode(List<AstNode> statements)
    {
        Statements = statements;
    }
}