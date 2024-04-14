namespace InterpreterForBasic.Domain;

public class PrintNode : AstNode
{
    public AstNode Expression { get; set; }

    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}