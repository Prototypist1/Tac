using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    // really an if not
    public class ElseOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        // right should have more validation
        public ElseOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.EmptyType);
        }
    }
}
