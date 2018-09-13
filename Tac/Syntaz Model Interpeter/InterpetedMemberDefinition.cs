using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMemberDefinition : MemberDefinition, IInterpeted
    {
        public InterpetedMemberDefinition(bool readOnly, ExplicitMemberName key, ITypeSource type) : base(readOnly, key, type)
        {
        }
    }
}