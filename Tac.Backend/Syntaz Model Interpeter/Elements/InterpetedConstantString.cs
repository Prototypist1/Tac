using System;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedConstantString : IInterpetedOperation<BoxedString>
    {
        public void Init(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }

        public IInterpetedResult<IInterpetedMember<BoxedString>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedMember<BoxedString>(new BoxedString(Value)));
        }
    }
}