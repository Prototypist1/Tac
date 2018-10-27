using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMultiplyOperation : WeakMultiplyOperation, IInterpeted
    {
        public InterpetedMultiplyOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeNumber(
                left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeNumber>().d *
                right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeNumber>().d));
        }

        internal static WeakMultiplyOperation MakeNew(IWeakCodeElement left, IWeakCodeElement right)
        {
            return new InterpetedMultiplyOperation(left, right);
        }
    }
}