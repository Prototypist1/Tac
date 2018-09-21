using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class MultiplyOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public MultiplyOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.NumberType);
        }
    }
}
