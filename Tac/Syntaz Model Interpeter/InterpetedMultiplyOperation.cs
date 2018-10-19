using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMultiplyOperation : MultiplyOperation, IInterpeted
    {
        public InterpetedMultiplyOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new NumberType(
                left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<NumberType>().d *
                right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<NumberType>().d));
        }

        internal static MultiplyOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedMultiplyOperation(left, right);
        }
    }
}