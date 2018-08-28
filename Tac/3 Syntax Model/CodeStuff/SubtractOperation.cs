using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class SubtractOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public SubtractOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override bool Equals(object obj) => obj is SubtractOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(IScope scope) => RootScope.NumberType;
    }
}
