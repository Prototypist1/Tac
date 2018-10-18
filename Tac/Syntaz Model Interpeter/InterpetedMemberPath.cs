//using Prototypist.LeftToRight;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Tac.Semantic_Model;

//namespace Tac.Syntaz_Model_Interpeter
//{
//    public class InterpetedMemberPath : Member, IInterpeted
//    {
//        public InterpetedMemberPath(int scopesUp, IReadOnlyList<IBox<MemberDefinition>> memberDefinitions) : base(scopesUp, memberDefinitions)
//        {
//        }

//        public InterpetedResult Interpet(InterpetedContext interpetedContext)
//        {
//            object at = interpetedContext.Scopes.Skip(ScopesUp).First();

//            foreach (var memberDefinition in MemberDefinitions)
//            {
//                at = at.Cast<IInterpetedScope>().GetMember(memberDefinition.GetValue().Key);
//            }

//            return InterpetedResult.Create(at);
//        }

//        internal static Member MakeNew(int scopesUp, IBox<MemberDefinition> memberDefinition)
//        {
//            return new InterpetedMemberPath(scopesUp, memberDefinition.ToArray());
//        }
//    }
//}