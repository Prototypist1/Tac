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
        
        public override bool Equals(object obj) => obj is ElseOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(IScope scope) => RootScope.EmptyType;
    }
}
