using System;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedConstantString : IInterpetedOperation<IBoxedString>
    {
        public void Init(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }

        public IInterpetedResult<IInterpetedMember<IBoxedString>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.StringMember(TypeManager.String(Value)));
        }
    }
}