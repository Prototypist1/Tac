using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedLessThanOperation : LessThanOperation, IInterpeted
    {
        public InterpetedLessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }


        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new BooleanType(
                left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<NumberType>().d <
                right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<NumberType>().d));
        }

        internal static LessThanOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedLessThanOperation(left, right);
        }
    }
}