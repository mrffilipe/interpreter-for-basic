namespace InterpreterForBasic.Domain;

public interface IVisitor
{
    void Visit(AssignmentNode node);
    void Visit(InputNode node);
    void Visit(HaltNode node);
    void Visit(BinaryOperationNode node);
    void Visit(VariableNode node);
    void Visit(ConstantNode node);
    void Visit(ConditionalNode node);
    void Visit(PrintNode node);
    void Visit(GotoNode node);
}