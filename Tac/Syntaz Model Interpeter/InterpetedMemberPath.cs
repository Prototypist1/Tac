using Prototypist.LeftToRight;
using System.Linq;
using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMemberPath : MemberPath, IInterpeted
    {
        public InterpetedMemberPath(int scopesUp, MemberDefinition[] memberDefinitions) : base(scopesUp, memberDefinitions)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            object at = interpetedContext.Scopes.Skip(ScopesUp).First();

            foreach (var name in MemberDefinitions)
            {
                at = at.Cast<IInterpetedScope>().GetMember(name.Key.Key);
            }

            return new InterpetedResult(at);
        }
    }
}