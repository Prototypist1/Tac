using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedAssignOperation : WeakAssignOperation, IInterpeted
    {
        public InterpetedAssignOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var res = right.Cast<IInterpeted>().Interpet(interpetedContext).Get<InterpetedMember>();

            res.Value = left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>();
            return InterpetedResult.Create(res);
        }

        internal static WeakAssignOperation MakeNew(IWeakCodeElement left, IWeakCodeElement right)
        {
            return new InterpetedAssignOperation(left, right);
        }
    }
}