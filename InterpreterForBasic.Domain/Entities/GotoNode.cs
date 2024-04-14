namespace InterpreterForBasic.Domain;

public class GotoNode : AstNode
{
    public int Target { get; set; }

    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}