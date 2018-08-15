using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class ElseOperation : BinaryOperation
    {
        public ElseOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override bool Equals(object obj) => obj is ElseOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}
