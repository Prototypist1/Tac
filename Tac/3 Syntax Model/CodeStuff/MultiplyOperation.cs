using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class MultiplyOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public MultiplyOperation(ICodeElement left, ICodeElement right) : base(left.TakeReferance(), right.TakeReferance())
        {
        }
        
        public override bool Equals(object obj) => obj is MultiplyOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(IScope scope) => RootScope.NumberType;
    }
}
