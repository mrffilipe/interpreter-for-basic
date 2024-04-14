namespace InterpreterForBasic.Domain;

public class VariableNode : AstNode
{
    public string Name { get; set; }

    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}