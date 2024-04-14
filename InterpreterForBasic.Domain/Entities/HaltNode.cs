namespace InterpreterForBasic.Domain;

public class HaltNode : AstNode
{
    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}