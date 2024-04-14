namespace InterpreterForBasic.Domain;

public class BinaryOperationNode : AstNode
{
    public AstNode Left { get; set; }
    public string Operator { get; set; }
    public AstNode Right { get; set; }

    public override void Accept(IVisitor visitor) => visitor.Visit(this);
}