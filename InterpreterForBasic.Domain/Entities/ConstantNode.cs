namespace InterpreterForBasic.Domain;

public class ConstantNode : AstNode
{
    public object Value { get; set; }

    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}