using System;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedConstantBool : IInterpetedOperation<BoxedBool>
    {
        public void Init(bool value)
        {
            this.Value = value;
        }

        public bool Value { get; private set; }

        public IInterpetedResult<IInterpetedMember<BoxedBool>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedMember<BoxedBool>(new BoxedBool(Value)));
        }
    }
}