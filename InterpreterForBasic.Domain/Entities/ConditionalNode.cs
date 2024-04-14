namespace InterpreterForBasic.Domain;

public class ConditionalNode : AstNode
{
    public AstNode Condition { get; set; }
    public List<AstNode> TrueBranch { get; set; } = new List<AstNode>();

    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}