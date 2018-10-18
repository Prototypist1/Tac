using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedLessThanOperation : LessThanOperation, IInterpeted
    {
        public InterpetedLessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }


        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(
                left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<double>() <
                right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<double>());
        }

        internal static LessThanOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedLessThanOperation(left, right);
        }
    }
}