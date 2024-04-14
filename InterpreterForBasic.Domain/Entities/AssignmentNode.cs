namespace InterpreterForBasic.Domain;

public class AssignmentNode : AstNode
{
    public VariableNode Variable { get; set; }
    public AstNode Expression { get; set; }

    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}