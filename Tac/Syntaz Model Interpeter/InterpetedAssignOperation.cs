using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedAssignOperation : AssignOperation, IInterpeted
    {
        public InterpetedAssignOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            // what if the lhs is a member?
            string s = 5;
            
            var res = left.Cast<IInterpeted>().Interpet(interpetedContext);
            
            right.Cast<IInterpeted>().Interpet(interpetedContext).Get<InterpetedMember>().Value = res;
            return InterpetedResult.Create(res);
        }

        internal static AssignOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedAssignOperation(left, right);
        }
    }
}