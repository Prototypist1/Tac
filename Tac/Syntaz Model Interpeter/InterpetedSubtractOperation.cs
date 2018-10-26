using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedSubtractOperation : WeakSubtractOperation, IInterpeted
    {
        public InterpetedSubtractOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeNumber(
                left.Cast<IInterpeted>().Interpet(interpetedContext).Get<RunTimeNumber>().d -
                right.Cast<IInterpeted>().Interpet(interpetedContext).Get<RunTimeNumber>().d));
        }

        internal static WeakSubtractOperation MakeNew(IWeakCodeElement left, IWeakCodeElement right)
        {
            return new InterpetedSubtractOperation(left, right);
        }
    }
}