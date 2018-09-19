using Prototypist.LeftToRight;
using System.Linq;
using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMemberPath : MemberPath, IInterpeted
    {
        public InterpetedMemberPath(int scopesUp, MemberDefinition memberDefinition) : base(scopesUp, memberDefinition)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            object at = interpetedContext.Scopes.Skip(ScopesUp).First();

            at = at.Cast<IInterpetedScope>().GetMember(MemberDefinition.Key.Key);
            
            return InterpetedResult.Create(at);
        }
    }
}