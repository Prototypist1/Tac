using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMemberDefinition : MemberDefinition, IInterpeted
    {
        public InterpetedMemberDefinition(bool readOnly, ExplicitMemberName key, ITypeDefinition type) : base(readOnly, key, type)
        {
        }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return new InterpetedResult(this);
        }
    }
}