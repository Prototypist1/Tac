using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMemberPath : Member, IInterpeted
    {
        public InterpetedMemberPath(int scopesUp, IBox<MemberDefinition> memberDefinition) : base(scopesUp, memberDefinition)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            object at = interpetedContext.Scopes.Skip(ScopesUp).First();

            at = at.Cast<IInterpetedScope>().GetMember(MemberDefinition.GetValue().Key);
            
            return InterpetedResult.Create(at);
        }

        internal static Member MakeNew(int scopesUp, IBox<MemberDefinition> memberDefinition)
        {
            return new InterpetedMemberPath(scopesUp, memberDefinition);
        }
    }
}