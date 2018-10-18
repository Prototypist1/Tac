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
            var res = right.Cast<IInterpeted>().Interpet(interpetedContext).Get<InterpetedMember>();

            res.Value = left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded();
            return InterpetedResult.Create(res);
        }

        internal static AssignOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedAssignOperation(left, right);
        }
    }
}