namespace InterpreterForBasic.Domain;

public abstract class AstNode
{
    public abstract void Accept(IVisitor visitor);
}