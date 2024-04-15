namespace InterpreterForBasic.Domain;

public class CompoundNode : AstNode
{
    public List<AstNode> Children { get; private set; }

    public CompoundNode()
    {
        Children = new List<AstNode>();
    }

    public CompoundNode(List<AstNode> children)
    {
        Children = children;
    }

    public override void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}