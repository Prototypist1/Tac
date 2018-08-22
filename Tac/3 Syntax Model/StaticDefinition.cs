using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public class IsDefininition : IReferanced, ICodeElement
    {

        public IsDefininition(MemberDefinition memberDef, ICodeElement valueSource) 
        {
            MemberDef = memberDef;
            ValueSource = valueSource.TakeReferance();
        }

        public MemberDefinition MemberDef { get; }
        public ICodeElement ValueSource { get; }

        public AbstractName Key => MemberDef.Key;

        public override bool Equals(object obj) => obj is IsDefininition other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}

