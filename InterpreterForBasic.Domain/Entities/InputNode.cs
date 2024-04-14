namespace InterpreterForBasic.Domain;

public class InputNode : AstNode
{
    public VariableNode Variable { get; set; }

    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}