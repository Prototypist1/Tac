using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMultiplyOperation : InterpetedBinaryOperation<IInterpetedData, IInterpetedData, IInterpetedBoolean>
    {
        // you are here.
        // control input types with constuctions
        // is a interpetedNumber a number member
        // a interpetedString a string member

        // maybe that would solve my member or value probelm
        // with value types you would have to copy a lot... any operation would have to save the result in a new

        public override IInterpetedResult<IInterpetedBoolean> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult<IInterpetedBoolean>.Create(new RuntimeNumber(
                Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RuntimeNumber>(interpetedContext).Value *
                Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RuntimeNumber>(interpetedContext).Value));
        }
    }
}